using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhishAnalyzer.Models
{
    internal class WhoisServer
    {
        public string Tld { get; set; }
        public string Server { get; set; }

        public WhoisServer(string tld, string server)
        {
            Tld = tld;
            Server = server;
        }
      
    }

}
