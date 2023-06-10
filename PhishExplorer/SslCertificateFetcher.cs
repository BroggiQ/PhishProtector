using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

public static class SslCertificateFetcher
{
    public static X509Certificate2 Fetch(string host, int port = 443)
    {
        X509Certificate2 certificate = null;

        using (TcpClient client = new TcpClient())
        {
            client.Connect(host, port);

            using (SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback((sender, certificate, chain, errors) => { return true; }), null))
            {
                try
                {
                    sslStream.AuthenticateAsClient(host);
                    certificate = new X509Certificate2(sslStream.RemoteCertificate);
                }
                catch (Exception)
                {
                    // Handle exceptions
                }
            }
        }

        return certificate;
    }
}
