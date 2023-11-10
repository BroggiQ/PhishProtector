
using Microsoft.AspNetCore.Mvc;
using PhishAnalyzer;
using PhishAnalyzer.Models;
 using System.Text.Json;

namespace PhishProtector.Windows.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AnalyzeController : ControllerBase
    {
        /// <summary>
        /// This is the main api that will analyze an url
        /// TODOThe next target is to analyze all information : url, html code, screens, certificate and create a reliability score for each criteria
        /// TODO add the age of the site
        /// </summary>
        /// <param name="url">The target url</param>
        /// <param name="screenBytes">The screenshot took by the browser extension</param>
        /// <param name="certificateInfoJson">The certificate received</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromForm] string url, [FromForm] string screenBytes, [FromForm] string certificateInfoJson)
        {
            try
            {
                if (!string.IsNullOrEmpty(url) && screenBytes != null)
                {
                    int base64StartIndex = screenBytes.IndexOf(',') + 1;
                    string base64String = screenBytes.Substring(base64StartIndex);

                    byte[] screenByteArray = Convert.FromBase64String(base64String);

                    // Certificate information analysis
                    CertificateInfo? certificateInfo = JsonSerializer.Deserialize<CertificateInfo>(certificateInfoJson);


                    //Get the score of this url
                    int score = Analyzer.GetScore(url, screenByteArray, certificateInfo);



                    if (score > 50)
                    {
                        return Ok(true);   
                    }
                    else
                    {
                        return Ok(false);
                    }




                }
                // The URL or screenshot of the website is old, making it impossible to process the site.
                return BadRequest("Url and/or Screen are empty");
            }
            catch (Exception ex)
            {
                //Internal error
                return StatusCode(500, ex.Message); 
             }
        }

    }
}
