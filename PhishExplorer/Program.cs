
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support;
using SeleniumExtras.WaitHelpers;
using System.Xml.Linq;
using OpenQA.Selenium.Interactions;

class Program
{ 
    private static  readonly string _screenFolder = @"C:\Users\Akutsu\source\repos\PhishProtector\PhishExplorer\Screens\";
    static void Main(string[] args)
    {
        // liste de sites web à visiter
        //TODO site livraison, edf, free, orange, impot, caf, ameli, banque, gmail ...
        List<string> sites = new List<string>() { "https://fr.yahoo.com/","https://store.steampowered.com", "https://www.amazon.fr",
            "https://www.ebay.fr/", "https://www.microsoft.com", "https://www.paypal.com",
             "https://www.netflix.com",
            "https://www.facebook.com/",  "https://www.google.com","https://www.apple.com" };

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


        IWebDriver driver = new FirefoxDriver(options);
 

        // parcourir chaque site et prendre une capture d'écran
        foreach (string site in sites)
        {
            driver.Navigate().GoToUrl(site);
 

            // accepter les cookies
            try
            {
 
                List<string> cookieTexts = new List<string>() {  "Autoriser les cookies essentiels et optionnels", "Accepter les cookies","Accepter tout", "Tout accepter", "Accepter les cookies", "Accepter et fermer" };
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                foreach (string cookieText in cookieTexts)
                {
                 IList<IWebElement> cookieButtons = driver.FindElements(By.XPath("//button[.='" + cookieText + "'] | //input[.='" + cookieText + "'] | //span[.='" + cookieText + "']"));
                     // IList<IWebElement> cookieButtons = driver.FindElements(By.XPath("//*[@value='" + cookieText + "']"));
                    if (cookieButtons.Count > 0)
                    {
                        IWebElement cookieButton = cookieButtons[0];
                        if (cookieButton != null)
                        {

                            if(cookieButton.Displayed && cookieButton.Enabled) {
                                wait.Until(ExpectedConditions.ElementToBeClickable(cookieButton));
                                cookieButton.Click();
                                  break;
                            }
                            else
                            {
                                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                js.ExecuteScript("arguments[0].scrollIntoView(true);", cookieButton);

                                Actions actions = new Actions(driver);
                                actions.MoveToElement(cookieButton).Perform();

                                cookieButton.Click();
                                break;


                            }
                        }
                    }

                };
            }
            catch(Exception ex)
            {
                Console.WriteLine("Consent error"+ ex.Message);
            }




            // prend la capture d'écran
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

            // extraire le nom du site sans le protocole et sans l'extension
            var uriSite = new Uri(site);
            string siteName = uriSite.Host;
            string screenName = string.Format("{0}_{1}.{2}", DateTime.Now.ToString("yyyy-MM-dd"), siteName,"png");


            string folderSitePath = Path.Combine(_screenFolder , siteName);
            if (!Directory.Exists(folderSitePath))
                Directory.CreateDirectory(folderSitePath);
            string imagePath = Path.Combine(folderSitePath, screenName );

            screenshot.SaveAsFile(imagePath, ScreenshotImageFormat.Png);


        }



        // ferme le navigateur
        driver.Quit();
    }
}