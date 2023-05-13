using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace PhishExplorer
{
    class WebCrawler : IDisposable
    {
        private IWebDriver _driver;

        public WebCrawler()
        {
            // create a custom profile to accept cookies
            FirefoxProfile profile = new FirefoxProfile();
            profile.SetPreference("network.cookie.cookieBehavior", 0);



            // configure the browser
            FirefoxOptions options = new FirefoxOptions();
            options.Profile = profile;

            options.AddArgument("--width=1280");
            options.AddArgument("--height=1024 ");


            // configure the browser in headless mode
            options.AddArgument("--headless");


            _driver = new FirefoxDriver(options);
        }

        /// <summary>
        /// The webcrawler go the specified url
        /// </summary>
        /// <param name="url"></param>
        public void NavigateTo(string url)
        {
            try
            {
                _driver.Navigate().GoToUrl(url);

                // We have to accept the cookies to remove a maximum of the popups
                //TODO create a list by country
                //I tried to automaticly accept the cookie but it doesn't work
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

        /// <summary>
        /// To have a sufficient number of screenshots we take the internal links of the site
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
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
                //TODO Need to check if the site is from the same base url
                if (!string.IsNullOrEmpty(href) && href.StartsWith(baseUrl))
                {
                    links.Add(href);
                }
            }*/

            return hrefs.Distinct().Take(20).ToList();
        }

        /// <summary>
        /// Take a screenshot of the website
        /// TODO define a format (size...) to optimize to ml.net model
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Dispose the webcrawler
        /// </summary>
        public void Dispose()
        {
            // Close the WebDriver
            _driver.Quit();
        }
    }

}
