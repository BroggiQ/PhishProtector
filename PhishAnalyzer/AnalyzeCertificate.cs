using PhishAnalyzer.Models;
using System.Text.RegularExpressions;

namespace PhishAnalyzer
{
    public static class AnalyzeCertificate
    {
 

        /// <summary>
        /// Check if the date of the certificate is valid
        /// </summary>
        /// <param name="certificateInfo"></param>
        /// <returns></returns>
        internal static bool IsDateValid(CertificateInfo certificateInfo)
        {
            bool isCertificateValid = false;
            if (certificateInfo.Validity != null) {
                // Check the expiration date of the certificate
                DateTimeOffset now = DateTimeOffset.UtcNow;

                isCertificateValid = now >= certificateInfo.Validity.NotBefore && now <= certificateInfo.Validity.NotAfter;
            }
            return isCertificateValid;
        }

        /// <summary>
        /// Check the issuer of the certificate
        /// </summary>
        /// <param name="certificateInfo"></param>
        /// <returns></returns>
        internal static bool IsTrustedIssuer(CertificateInfo certificateInfo)
        {
            bool isTrustedIssuer = false;
            if(certificateInfo.Issuer != null)
            {
                string issuer = certificateInfo.Issuer;
                // List of trusted issuers
                //TODO Mozilla Included CA Certificate List : https://wiki.mozilla.org/CA/Included_Certificates

            }
            return isTrustedIssuer;
        }

        /// <summary>
        /// Check the subject of the certificate
        /// </summary>
        /// <param name="certificateInfo"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static bool IsSubjectValid(CertificateInfo certificateInfo, string url)
        {
            bool isSubjectValid = false;
            if(certificateInfo.Subject != null) { 
            string subject = certificateInfo.Subject;
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
            }
            return isSubjectValid;
        }
    }
}
