namespace PhishAnalyzer
{
    public class UriComparison
    {
        /// <summary>
        /// Comparison of two urls to check subdomains examples www.google.com with news.google.com or google.fr...
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
                // If both urls are identical, then we are on the official site
                if (sldAndTld1.Equals(sldAndTld2, StringComparison.OrdinalIgnoreCase))
                    return true;
                int levenshteinDistance = CalculateLevenshteinDistance(sldAndTld1, sldAndTld2);
                // TODO 3 is chosen arbitrarily, to be further studied
                // TODO What about sites with totally bogus urls
                // TODO What about official sites like subsidiaries
                // If the difference is less than 3, it could be a phishing site
                if (levenshteinDistance > 3)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// Get the second-level domain (SLD) and top-level domain (TLD) from a url
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


        public static int CalculateLevenshteinDistance(string s, string t)
        {
            int sLen = s.Length;
            int tLen = t.Length;
            int[,] d = new int[sLen + 1, tLen + 1];

            for (int i = 0; i <= sLen; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= tLen; j++)
            {
                d[0, j] = j;
            }

            for (int i = 1; i <= sLen; i++)
            {
                for (int j = 1; j <= tLen; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(
                            d[i - 1, j] + 1, // deletion
                            d[i, j - 1] + 1), // insertion
                        d[i - 1, j - 1] + cost); // substitution
                }
            }

            return d[sLen, tLen];
        }
    }
}
