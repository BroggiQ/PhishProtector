
using PhishExplorer;

class Program
{
    private static string _screenFolder = "";
    static void Main(string[] args)
    {



        // liste de sites web à visiter
        //TODO site livraison, edf, free, orange, impot, caf, ameli, banque, gmail ...
        List<string> sites = SiteList.GetSites();




        using (var webCrawler = new WebCrawler())
        {
            var screenshotTaker = new ScreenshotTaker();

            foreach (string site in sites)
            {
                int counter = 0;
                webCrawler.NavigateTo(site);
                var mainScreenshot = webCrawler.TakeScreenshot();
                if (mainScreenshot != null) {
                    screenshotTaker.SaveScreenshot(mainScreenshot, site, counter);
                }

                var internalLinks = webCrawler.GetInternalLinks(site);
               
                foreach (string internalLink in internalLinks)
                {
                    counter++;
                    webCrawler.NavigateTo(internalLink);

                    var screenshot = webCrawler.TakeScreenshot();
                    if (screenshot != null) {
                        screenshotTaker.SaveScreenshot(screenshot, site, counter);

                    }
                   

                }
            }
        }


 
    }
}