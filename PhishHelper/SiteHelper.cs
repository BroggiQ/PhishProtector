using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhishProtector.Helper
{
    public static class SiteHelper
    {
        public static string GetHostFromUrl(string url)
        {
            if (url.StartsWith("www.") || url.StartsWith("http:") || url.StartsWith("https:"))
            {
                var uriSite = new Uri(url);
                return uriSite.Host;

            }
            else
                return url;
        }
    }
}
