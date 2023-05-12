
namespace PhishAnalyzer.Models
{
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
