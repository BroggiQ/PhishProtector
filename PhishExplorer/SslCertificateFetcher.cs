using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

public static class SslCertificateFetcher
{
    public static X509Certificate2 Fetch(string host, int port = 443)
    {
        try
        {
            X509Certificate2 certificate = null;

            using (TcpClient client = new TcpClient())
            {
                client.Connect(host, port);

                using (SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback((sender, certificate, chain, errors) => { return true; }), null))
                {

                    sslStream.AuthenticateAsClient(host);
                    certificate = new X509Certificate2(sslStream.RemoteCertificate);

                }
            }

            return certificate;
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Failed to connect: " + ex.Message);
            return null;
        }
    }
}
