using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhishExplorer
{
    class WebCrawler : IDisposable
    {
        private IWebDriver _driver;

        public WebCrawler()
        {
            // crée un profil personnalisé pour accepter les cookies
            FirefoxProfile profile = new FirefoxProfile();
            profile.SetPreference("network.cookie.cookieBehavior", 0);



            // configure le navigateur
            FirefoxOptions options = new FirefoxOptions();
            options.Profile = profile;

            options.AddArgument("--width=1280");
            options.AddArgument("--height=1024 ");


            // configure le navigateur en mode headless
            options.AddArgument("--headless");


            _driver = new FirefoxDriver(options);
        }

        public void NavigateTo(string url)
        {
            try
            {
                _driver.Navigate().GoToUrl(url);
                // accepter les cookies


                List<string> cookieTexts = new List<string>() { "Autoriser les cookies essentiels et optionnels", "Accepter les cookies", "Accepter tout", "Tout accepter", "Accepter les cookies", "Accepter et fermer" };
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                foreach (string cookieText in cookieTexts)
                {
                    IList<IWebElement> cookieButtons = _driver.FindElements(By.XPath("//button[.='" + cookieText + "'] | //input[.='" + cookieText + "'] | //span[.='" + cookieText + "']"));
                    // IList<IWebElement> cookieButtons = driver.FindElements(By.XPath("//*[@value='" + cookieText + "']"));
                    if (cookieButtons.Count > 0)
                    {
                        IWebElement cookieButton = cookieButtons[0];
                        if (cookieButton != null)
                        {

                            if (cookieButton.Displayed && cookieButton.Enabled)
                            {
                                wait.Until(ExpectedConditions.ElementToBeClickable(cookieButton));
                                cookieButton.Click();
                                break;
                            }
                            else
                            {
                                IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                                js.ExecuteScript("arguments[0].scrollIntoView(true);", cookieButton);

                                Actions actions = new Actions(_driver);
                                actions.MoveToElement(cookieButton).Perform();

                                cookieButton.Click();
                                break;


                            }
                        }
                    }

                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Consent error" + ex.Message);
            }
        }

        public List<string> GetInternalLinks(string baseUrl)
        {
            var links = new List<string>();
            var anchorTags = _driver.FindElements(By.TagName("a"));
            var uriSite = new Uri(baseUrl);

            var hrefs = anchorTags.Select(anchor => anchor.GetAttribute("href")).ToList();
            hrefs = hrefs.Where(h => h != null && h.StartsWith("http") && h.Contains(uriSite.Host) && baseUrl != h).ToList();
            /*foreach (var anchorTag in anchorTags)
            {
                var href = anchorTag.GetAttribute("href");
                //On précse que l'on reste sur le meme site
                if (!string.IsNullOrEmpty(href) && href.StartsWith(baseUrl))
                {
                    links.Add(href);
                }
            }*/

            return hrefs.Distinct().Take(20).ToList();
        }

        public Screenshot TakeScreenshot()
        {
            try
            {

                return ((ITakesScreenshot)_driver).GetScreenshot();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Consent error" + ex.Message);

                return null;
            }
        }

        public void Dispose()
        {
            // Fermer le WebDriver
            _driver.Quit();
        }
    }

}
