

namespace PhishExplorer
{
    /// <summary>
    /// list of websites most at risk of being the target of phshing
    /// TODO create a list by country
    /// Move this list in another format sql lite?
    /// </summary>
    static class SiteList
    {
        public static List<string> GetSites()
        {
            return new List<string>
            {
                "https://fr.yahoo.com/",
                "https://store.steampowered.com", 
                "https://www.amazon.fr",
                "https://www.ebay.fr/",
                "https://www.microsoft.com", 
                "https://www.paypal.com",
                "https://www.netflix.com",
                "https://www.facebook.com/", 
                "https://www.google.com",
                "https://www.apple.com"
            };
        }
    }
}
