using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhishExplorer
{
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
