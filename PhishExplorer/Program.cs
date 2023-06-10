
using PhishExplorer;
using PhishHelper;
using PhishHelper.Models;
using System.Security.Cryptography.X509Certificates;

class Program
{
    private static string _screenFolder = "";
    static void Main(string[] args)
    {


        // list of websites to visit
        // TODO delivery site, edf, free, orange, tax, caf, ameli, bank, gmail ...

        List<string> sites = SiteList.GetTrustedSites();




        using (var webCrawler = new WebCrawler())
        {
            var screenshotTaker = new ScreenshotTaker();

            foreach (string site in sites)
            {
                //On récupère le certificat
                //TODO on va alimenter le modele (url / certificat / whois)
                //Peut etre qu'il faudra ne faire 3 modele ml.net?

                X509Certificate2 certificate = SslCertificateFetcher.Fetch(site);
                Whois whois = WhoisFetcher.GetWhois(site);


                int counter = 0;
                webCrawler.NavigateTo(site);
                var mainScreenshot = webCrawler.TakeScreenshot();
                if (mainScreenshot != null)
                {
                    screenshotTaker.SaveScreenshot(mainScreenshot, site, counter);
                }

                var internalLinks = webCrawler.GetInternalLinks(site);

                foreach (string internalLink in internalLinks)
                {
                    counter++;
                    webCrawler.NavigateTo(internalLink);

                    var screenshot = webCrawler.TakeScreenshot();
                    if (screenshot != null)
                    {
                        screenshotTaker.SaveScreenshot(screenshot, site, counter);

                    }


                }
            }
        }



    }
}