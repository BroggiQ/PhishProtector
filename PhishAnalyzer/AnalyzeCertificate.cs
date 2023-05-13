using PhishAnalyzer.Models;
using System.Text.RegularExpressions;

namespace PhishAnalyzer
{
    public static class AnalyzeCertificate
    {
        /// <summary>
        /// This have to analyze the certificate, the target is to give a fiability score
        /// </summary>
        /// <param name="certificateInfo"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsReliableCertificate(CertificateInfo certificateInfo, string url)
        {
            // Check the issuer of the certificate
            bool isIssuerTrusted = IsTrustedIssuer(certificateInfo.Issuer);

            // Check the expiration date of the certificate
            DateTimeOffset now = DateTimeOffset.UtcNow;
            bool isCertificateValid = now >= certificateInfo.Validity.NotBefore && now <= certificateInfo.Validity.NotAfter;

            // Check the subject of the certificate
            bool isSubjectValid = IsSubjectValid(certificateInfo.Subject, url);

            // If all checks are correct, the certificate is considered safe
            return isIssuerTrusted && isCertificateValid && isSubjectValid;
        }

        /// <summary>
        /// Check if the certificate from a trusted issuer
        /// TODO get a white list of the trusted issuer
        /// </summary>
        /// <param name="issuer"></param>
        /// <returns></returns>
        private static bool IsTrustedIssuer(string issuer)
        {
            // List of trusted issuers
            //TODO Mozilla Included CA Certificate List : https://wiki.mozilla.org/CA/Included_Certificates
            return true;
        }

        /// <summary>
        /// Check if the Subject is valid
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private static bool IsSubjectValid(string subject, string url)
        {
            // Extract the domain name from the URL
            var match = Regex.Match(url, @"https?://(www\.)?([-\w]+(\.\w[-\w]*)+)");
            if (!match.Success)
            {
                return false;
            }

            string domain = match.Groups[2].Value;

            // Extract the domain name from the subject (CN)
            match = Regex.Match(subject, @"CN=(\*\.)?([-\w]+(\.\w[-\w]*)+)");
            if (match.Success)
            {
                string subjectDomain = match.Groups[2].Value;
                if (domain.Equals(subjectDomain, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // Check alternative domain names (SAN)
            // Note: This simplified implementation assumes that the subject contains only one SAN DNS entry.
            // For a more robust implementation, you may need to parse multiple SAN entries.
            match = Regex.Match(subject, @"DNS:(\*\.)?([-\w]+(\.\w[-\w]*)+)");
            if (match.Success)
            {
                string sanDomain = match.Groups[2].Value;
                if (domain.Equals(sanDomain, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
