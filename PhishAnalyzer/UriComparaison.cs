using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhishAnalyzer
{
    public class UriComparaison
    {
        /// <summary>
        /// Comparaison de deux urls afin de vérifier les sous domaines exemples www.google.com avec news.google.com ou google.fr...
        /// </summary>
        /// <param name="url1"></param>
        /// <param name="url2"></param>
        /// <returns></returns>
        public static bool IsSameOfficialSite(string url1, string url2)
        {
            try
            {
                 string sldAndTld1 = GetSldAndTldFromUrl(url1);
                string sldAndTld2 = GetSldAndTldFromUrl(url2);

                return sldAndTld1.Equals(sldAndTld2, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Récupérer le domaine de second niveau (SLD) et le domaine de premier niveau (TLD) à partir d'une url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string GetSldAndTldFromUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                url = "http://" + url;
                uri = new Uri(url);
            }

            string[] domainParts = uri.Host.Split('.');
            string sldAndTld = string.Join(".", domainParts.Reverse().Take(2).Reverse().ToArray());


            return sldAndTld;
        }
    }
}
