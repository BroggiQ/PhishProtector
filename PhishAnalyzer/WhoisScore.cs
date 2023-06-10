using NumSharp.Utilities;
using PhishHelper;
using PhishHelper.Models;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace PhishAnalyzer
{
    internal class WhoisScore
    {

        /// <summary>
        /// For the .com need to use: Verisign
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static int GetWhoisScore(string url)
        {
            int score = 0;

            Whois whois = WhoisFetcher.GetWhois(url);


            if (whois.CreatedDate.HasValue)
            {
                if (whois.CreatedDate <= DateTime.Now.AddMonths(-6))
                    score += 5;
            }
            if (whois.ExpirationDate.HasValue)
            {
                if (whois.ExpirationDate >= DateTime.Now.AddMonths(6))
                    score += 5;
            }
            if (whois.Status == "active")
            {
                //Active = currently in service
                //Hold = possibility of problem
                score += 5;
            }
            //TODO see contact , registrar and nserver


            return score;
        }
    }
}
