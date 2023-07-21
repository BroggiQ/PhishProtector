using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using PhishProtector.Helper;
using SeleniumExtras.WaitHelpers;
using System.Net;

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

        public void NavigateTo(string url)
        {
            try
            {
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    url = "https://" + url;
                }
                Console.WriteLine("Navigating to: " + url);
                _driver.Navigate().GoToUrl(url);

                // accept cookies
                List<string> cookieTexts = new List<string>() { "autoriser les cookies essentiels et optionnels", "accepter les cookies", "accepter tout", "tout accepter", "accepter les cookies", "accepter et fermer" };
                WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                foreach (string cookieText in cookieTexts)
                {
                    IList<IWebElement> cookieButtons = _driver.FindElements(By.XPath("//button[contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '" + cookieText + "')] | //input[contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '" + cookieText + "')] | //span[contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '" + cookieText + "')]"));                    // IList<IWebElement> cookieButtons = driver.FindElements(By.XPath("//*[@value='" + cookieText + "']"));
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
                               // string elementId = cookieButton.GetAttribute("id");
                              // js.ExecuteScript("document.getElementById("+ elementId+").click();");
                                js.ExecuteScript("arguments[0].click();", cookieButton);
                                //js.ExecuteScript("arguments[0].scrollIntoView(true);", cookieButton);

                            //    Actions actions = new Actions(_driver);
                              //  actions.MoveToElement(cookieButton).Perform();

                            //    cookieButton.Click();
                                break;


                            }
                        }
                    }

                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("NavigateTo " + ex.Message);
            }
        }

        public List<string> GetInternalLinks(string baseUrl)
        {
            try
            {
                var links = new List<string>();
                /*      WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
                      wait.Until(drv => drv.FindElements(By.TagName("a")).Count > 0);

                      var hrefs = _driver.FindElements(By.TagName("a")).Select(anchor => anchor.GetAttribute("href"))
                                      .ToList(); 

                          string hostName = SiteHelper.GetHostFromUrl(baseUrl);

          */
                string hostName = SiteHelper.GetHostFromUrl(baseUrl);

                var anchorTags = _driver.FindElements(By.TagName("a"));
                List<string> hrefs = new List<string>();



                //var hrefs = anchorTags.Select(anchor => anchor.GetAttribute("href")).ToList();


                foreach (var anchor in anchorTags)
                {
                    try
                    {
                        hrefs.Add(anchor.GetAttribute("href"));
                    }
                    catch (StaleElementReferenceException)
                    {
                    }
                    catch (Exception ex)
                    {
                        // handle any other type of exception
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }
 

                hrefs = hrefs.Where(h => h != null && h.StartsWith("http") && h.Contains(hostName) && baseUrl != h).ToList();
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
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Timeout. Current page content:");
                Console.WriteLine(_driver.PageSource);
                throw;  // re-throw the exception
            }
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
            // Close the WebDriver
            _driver.Quit();
        }
    }

}
