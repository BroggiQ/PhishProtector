using Microsoft.VisualBasic;
using PhishExplorer;
using PhishHelper;
using PhishHelper.Models;
using System;
using System.Security.Cryptography.X509Certificates;
using Whois;
using System.Collections.Generic;
using System.Linq;
using System.IO;

class Program
{
    // List of trusted sites to visit
    private static List<string> sites = SiteList.GetTrustedSites();

    static void Main(string[] args)
    {
        // Use a web crawler to visit sites and take screenshots
        using (var webCrawler = new WebCrawler())
        {
            var screenshotTaker = new ScreenshotTaker();
            List<string> lines = GenerateHeaders();
            lines.AddRange(VisitSites(webCrawler, screenshotTaker));
            SaveResults(lines);
        }
    }

    /// <summary>
    /// Generate the headers for the CSV file.
    /// </summary>
    private static List<string> GenerateHeaders()
    {
        return new List<string>
        {
            string.Join(",", new string[]
            {
                "Site",
                "URL Length",
                "Subdomain Count",
                "Special Character Count",
                "Months Since Creation",
                "Registrant Name",
                "Registrant Address",
                "Certificate Issuer Name",
                "Certificate Country",
                "Certificate Validity",
                "Label"
            })
        };
    }

    /// <summary>
    /// Visit each site in the list and collect information.
    /// </summary>
    private static List<string> VisitSites(WebCrawler webCrawler, ScreenshotTaker screenshotTaker)
    {
        var results = new List<string>();
        foreach (string site in sites)
        {
            var siteResult = VisitSite(webCrawler, screenshotTaker, site);
            results.Add(siteResult);
        }
        return results;
    }

    /// <summary>
    /// Visit a single site, collect information, and take screenshots.
    /// </summary>
    private static string VisitSite(WebCrawler webCrawler, ScreenshotTaker screenshotTaker, string site)
    {
        string[] data = new string[11];

        // Get site information
        data[0] = site.Trim();
        data[1] = site.Length.ToString();
        data[2] = (site.Split('.').Length - 2).ToString();
        data[3] = site.Count(c => !char.IsLetterOrDigit(c) && c != '.' && c != '/' && c != ':').ToString();

        // Get WHOIS information
        var whois = new WhoisLookup();
        WhoisResponse response = null;
        try
        {
            response = whois.Lookup(site);
            data[4] = GetMonthsSinceCreation(response).ToString();
            data[5] = response.Registrant?.Name?.Trim() ?? "";
            data[6] = response.Registrant?.Address?.Last()?.Trim() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting WHOIS information for site {site}: {ex.Message}");
            // Fill with default values if there's an error
            data[4] = "0";
            data[5] = "";
            data[6] = "";
        }

        // Get SSL certificate information
        using (var certificate = SslCertificateFetcher.Fetch(site))
        {
            data[7] = certificate?.IssuerName?.Name?.Trim() ?? "";
            data[8] = GetCertificateCountry(certificate);
            data[9] = IsCertificateValid(certificate).ToString();
        }

        // The label for the site (always "Fiable" in this case)
        data[10] = "Fiable";

        var line = string.Join(",", data);
        CaptureScreenshots(webCrawler, screenshotTaker, site);

        return line;
    }

    /// <summary>
    /// Get the number of months since the domain was registered.
    /// </summary>
    private static int GetMonthsSinceCreation(WhoisResponse response)
    {
        if (response.Registered.HasValue)
        {
            return 12 * (DateTime.Now.Year - response.Registered.Value.Year) + DateTime.Now.Month - response.Registered.Value.Month;
        }
        return 0;
    }

    /// <summary>
    /// Get the country from the SSL certificate.
    /// </summary>
    private static string GetCertificateCountry(X509Certificate2 certificate)
    {
        if (certificate != null)
        {
            string[] certificateSubjects = certificate.Subject.Split(',');
            var certificateSubjectCountry = certificateSubjects.FirstOrDefault(x => x.StartsWith("C="));
            return certificateSubjectCountry?.Replace("C=", "").Trim() ?? "";
        }
        return "";
    }

    /// <summary>
    /// Check if the SSL certificate is valid.
    /// </summary>
    private static bool IsCertificateValid(X509Certificate2 certificate)
    {
        if (certificate != null)
        {
            return DateTime.Now >= certificate.NotBefore && DateTime.Now <= certificate.NotAfter;
        }
        return false;
    }

    /// <summary>
    /// Capture screenshots of the site and its internal links.
    /// </summary>
    private static void CaptureScreenshots(WebCrawler webCrawler, ScreenshotTaker screenshotTaker, string site)
    {
        int counter = 0;
        webCrawler.NavigateTo(site);
        SaveScreenshot(webCrawler, screenshotTaker, site, counter);
        var internalLinks = webCrawler.GetInternalLinks(site);

        foreach (string internalLink in internalLinks)
        {
            counter++;
            webCrawler.NavigateTo(internalLink);
            SaveScreenshot(webCrawler, screenshotTaker, site, counter);
        }
    }

    /// <summary>
    /// Save a screenshot of a page.
    /// </summary>
    private static void SaveScreenshot(WebCrawler webCrawler, ScreenshotTaker screenshotTaker, string site, int counter)
    {
        var screenshot = webCrawler.TakeScreenshot();
        if (screenshot != null)
        {
            screenshotTaker.SaveScreenshot(screenshot, site, counter);
        }
    }

    /// <summary>
    /// Save the results to a CSV file.
    /// </summary>
    private static void SaveResults(List<string> lines)
    {
#if DEBUG
        string path = @"C:\Users\Akutsu\Desktop\Phishing\sitesheaders.txt";
#else
        string path = Path.Combine(Directory.GetCurrentDirectory(), "sitesheaders.txt");
#endif
        File.WriteAllLines(path, lines);
    }
}
