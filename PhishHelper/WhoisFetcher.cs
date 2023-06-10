using PhishHelper.Models;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace PhishHelper
{
    public static class WhoisFetcher
    {
        //TODO change this format
        public static List<WhoisServer> WhoisServers = new List<WhoisServer>
            {
                new WhoisServer("com", "whois.verisign-grs.com"),
                new WhoisServer("net", "whois.verisign-grs.com"),
                new WhoisServer("org", "whois.pir.org"),
                new WhoisServer("io", "whois.nic.io"),
                new WhoisServer("ai", "whois.ai"),
                new WhoisServer("co", "whois.nic.co"),
                new WhoisServer("uk", "whois.nic.uk"),
                new WhoisServer("de", "whois.denic.de"),
                new WhoisServer("fr", "whois.nic.fr"),
                new WhoisServer("it", "whois.nic.it"),
                new WhoisServer("ru", "whois.tcinet.ru"),
                new WhoisServer("br", "whois.registro.br"),
                new WhoisServer("au", "whois.audns.net.au"),
                new WhoisServer("ca", "whois.cira.ca"),
                new WhoisServer("cn", "whois.cnnic.cn"),
                new WhoisServer("jp", "whois.jprs.jp"),
                new WhoisServer("kr", "whois.kr"),
                new WhoisServer("in", "whois.registry.in"),
                new WhoisServer("eu", "whois.eu"),
                new WhoisServer("us", "whois.nic.us"),
            };

        /// <summary>
        /// For the .com need to use: Verisign
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Whois GetWhois(string url)
        {

            Whois whois = new Whois();

            var uri = new Uri(url);
            var host = uri.Host;
            var domain = string.Join(".", host.Split('.').Reverse().Take(2).Reverse().ToArray());
            string tld = uri.GetComponents(UriComponents.Host, UriFormat.SafeUnescaped);
            string[] tldParts = tld.Split('.');
            tld = tldParts[tldParts.Length - 1];


            string? whoisServer = WhoisServers.FirstOrDefault(w => w.Tld == tld)?.Server;
            //we check that the tld is managed in our list
            if (!string.IsNullOrEmpty(tld))
            {
                int portNumber = 43;

                TcpClient tcpClinetWhois = new TcpClient(whoisServer, portNumber);
                NetworkStream networkStreamWhois = tcpClinetWhois.GetStream();
                BufferedStream bufferedStreamWhois = new BufferedStream(networkStreamWhois);
                StreamWriter streamWriter = new StreamWriter(bufferedStreamWhois);

                streamWriter.WriteLine(domain);
                streamWriter.Flush();

                StreamReader streamReaderReceive = new StreamReader(bufferedStreamWhois);
                DateTime createdDate;
                DateTime expirationDate;
                string status;

                while (!streamReaderReceive.EndOfStream)
                {
                    string line = streamReaderReceive.ReadLine();
                    List<string> lineSplitted = line.Split(':').ToList();
                    if (lineSplitted.Count() > 1)
                    {
                        if (lineSplitted[0].ToLower().Trim() == "created")
                        {
                            lineSplitted.RemoveAt(0);
                            string date = string.Join(':', lineSplitted);
                            DateTime.TryParse(date, out createdDate);
                            whois.CreatedDate = createdDate;
                        }
                        else if (lineSplitted[0].ToLower().Trim() == "expiry date")
                        {
                            lineSplitted.RemoveAt(0);
                            string date = string.Join(':', lineSplitted);
                            DateTime.TryParse(date, out expirationDate);
                            whois.ExpirationDate = expirationDate;

                        }
                        else if (lineSplitted[0].ToLower().Trim() == "status")
                        {
                            status = lineSplitted[1].Trim();
                            //Active = currently in service
                            //Hold = possibility of problem
                            whois.Status = status.ToLower().Trim();
                        }
                        //TODO see contact , registrar and nserver
                    }
                }
            }
            return whois;
        }
    }
}
