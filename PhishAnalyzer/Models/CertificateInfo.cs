
namespace PhishAnalyzer.Models
{
    /// <summary>
    /// Certificate Info of the website get from the browser extension
    /// </summary>
    public class CertificateInfo
    {
        public string Issuer { get; set; }
        public string SerialNumber { get; set; }
        public string Subject { get; set; }
        public string SubjectPublicKeyInfoDigest { get; set; }
        public Validity Validity { get; set; }
    }

    public class Validity
    {
        public DateTimeOffset NotBefore { get; set; }
        public DateTimeOffset NotAfter { get; set; }
    }
}
