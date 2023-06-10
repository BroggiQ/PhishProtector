using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhishHelper.Models
{
    public class Whois
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Status { get; set; }
    }
}
