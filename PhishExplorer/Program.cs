
using PhishExplorer;

class Program
{
    private static string _screenFolder = "";
    /// <summary>
    /// PhishExplorer goes through the list of sites most at risk of being the target of phishing
    /// It takes many screens of these website to feet the AI ml.net
    /// TODO We have to get the html code too
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        // list of websites to visit
        // TODO delivery site, edf, free, orange, tax, caf, ameli, bank, gmail ...
        // TODO create a list by country
        List<string> sites = SiteList.GetSites();


        using (var webCrawler = new WebCrawler())
        {
            var screenshotTaker = new ScreenshotTaker();

            foreach (string site in sites)
            {
                int counter = 0;
                webCrawler.NavigateTo(site);
                var mainScreenshot = webCrawler.TakeScreenshot();
                if (mainScreenshot != null)
                {
                    screenshotTaker.SaveScreenshot(mainScreenshot, site, counter);
                }
                //We get a definite number of internal links to have enough screens of the website
                var internalLinks = webCrawler.GetInternalLinks(site);

                foreach (string internalLink in internalLinks)
                {
                    counter++;
                    //The webcrawler go to the website
                    webCrawler.NavigateTo(internalLink);
                    //The webcrawler takes a screen of the website
                    var screenshot = webCrawler.TakeScreenshot();
                    if (screenshot != null)
                    {
                        //We save the screen
                        screenshotTaker.SaveScreenshot(screenshot, site, counter);

                    }

                }
            }
        }

    }
}