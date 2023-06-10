using PhishAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhishAnalyzer
{
    public static class Analyzer
    {
        /// <summary>
        /// 25 points certificate
        /// 25 points screen
        /// 15 points urls
        /// 25 points html code source
        /// 10 WHOIS
        /// </summary>
        /// <param name="url"></param>
        /// <param name="screenByteArray"></param>
        /// <param name="certificateInfo"></param>
        /// <returns></returns>
        public static int GetScore(string url, byte[] screenByteArray, CertificateInfo? certificateInfo)
        {
            int globalScore = 0;

            //Certificate score is at 25 max
            int scoreCertificate = 0;
            //A missing certificate as 0 for value
            if (certificateInfo != null) {
                if (AnalyzeCertificate.IsDateValid(certificateInfo))
                    scoreCertificate += 10;
                if(AnalyzeCertificate.IsSubjectValid(certificateInfo, url))
                    scoreCertificate += 10;
                if(AnalyzeCertificate.IsTrustedIssuer(certificateInfo))
                    scoreCertificate += 5;
            }
            globalScore += scoreCertificate;

            SiteClassification.ModelInput modelInput = new SiteClassification.ModelInput()
            {
                ImageSource = screenByteArray,
            };

            // Load model and predict output
            var resultScreen = SiteClassificationManager.Predict(modelInput);
            //25 points if the screen doesn't look at a website on the watching list of website
            int screenScore = 0;
            if (resultScreen == null)
                screenScore = 25;
            else
            {
                //The screen look close of an official site
                if (UriComparison.IsSameOfficialSite(url, resultScreen.PredictedLabel))
                {
                    // The website appears to be under surveillance and the URL is valid.
                    screenScore = 25;
                }
                else
                {
                    // The website appears to be under surveillance, but the URL is different.
                    // Improve this system to consider subsidiaries as well...
                    screenScore = 0;
                }
            }

            globalScore += screenScore;


            //15 points if the url doesn't look close to a website on the watching list of website
            /* Modele ML.net
            URL	Longueur URL	Nb Sous-domaines	Caractères spéciaux	Mots suspects	Âge du domaine	Étiquette
            www.google.com	14	2	0	0	22	Fiable
            www.bad-url-example.com	22	3	0	0	1	Non fiable
            good-site.com	12	1	0	0	5	Fiable
            Fusionné certificat et url ?
             */
            int urlScore = 0;
            //TODO URL score
            globalScore += urlScore;

            //25 points for the source code
            //Modele d'analyse du code source
            int sourceCodeScore = 0;
            //TODO html code source score
            globalScore += sourceCodeScore;

            //10 points for a reliable from whois
            int whoisScore = WhoisScore.GetWhoisScore(url);
            globalScore += whoisScore;




            return globalScore;
        }
    }
}
