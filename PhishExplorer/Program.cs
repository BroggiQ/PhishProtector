
using Microsoft.VisualBasic;
using PhishExplorer;
using PhishHelper;
using PhishHelper.Models;
using System;
using System.Security.Cryptography.X509Certificates;
using Whois;

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

                var whois = new WhoisLookup();
                var uri = new Uri(site);
                var host = uri.Host;
                var domain = string.Join(".", host.Split('.').Reverse().Take(2).Reverse().ToArray());

                var response = whois.Lookup(domain);

                X509Certificate2 certificate = SslCertificateFetcher.Fetch(domain);

                // Whois whois = WhoisFetcher.GetWhois(site);
                //string[] lines = { "URL", "Longueur URL", "Nb Sous-domaines", "Caractères spéciaux", "Âge du domaine", "Étiquette" };
                string urlLength = site.Length.ToString();
                string subdomainCount = (site.Split('.').Length - 2).ToString();
                string specialCharCount = site.Count(c => !char.IsLetterOrDigit(c) && c != '.' && c != '/' && c != ':').ToString();
                string numberMonthSinceCreation = "";
                if (response.Registered.HasValue)
                    numberMonthSinceCreation =  ( 12 * (DateTime.Now.Year -response.Registered.Value.Year   ) + DateTime.Now.Month - response.Registered.Value.Month ).ToString();
                //Nom du propriétaire
                string registrantName = response.Registrant.Name;
                //Pays du proprietaire
                string registrantAddress = response.Registrant.Address.Last();
                //Validité du certificat SSL 
                bool isCertificateValid = DateTime.Now >= certificate.NotBefore && DateTime.Now <= certificate.NotAfter;
                //Pays du proprietaire du certificat
                string[] certificateSubjects = certificate.Subject.Split(',');
                var certificateSubjectCountry = certificateSubjects.FirstOrDefault(x=>x.StartsWith("C="));
                string certificateCountry = certificateSubjectCountry.Replace("C=","");

                string[] lines = { site, urlLength, subdomainCount, specialCharCount, numberMonthSinceCreation,registrantName,registrantAddress , certificate.IssuerName.Name, certificateCountry, isCertificateValid.ToString(), "Fiable" };






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