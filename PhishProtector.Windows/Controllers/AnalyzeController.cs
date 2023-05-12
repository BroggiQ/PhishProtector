
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
        [HttpPost]
        //  public IActionResult Post([FromForm] string url, [FromForm] byte[] screenBytes)
        public IActionResult Post([FromForm] string url, [FromForm] string screenBytes, [FromForm] string certificateInfoJson)
        {
            try
            {
                if (!string.IsNullOrEmpty(url) && screenBytes != null)
                {
                    int base64StartIndex = screenBytes.IndexOf(',') + 1;
                    string base64String = screenBytes.Substring(base64StartIndex);

                    byte[] imageByteArray = Convert.FromBase64String(base64String);

                    // Certificate information analysis
                    var certificateInfo = JsonSerializer.Deserialize<CertificateInfo>(certificateInfoJson);
                    bool isCertificateSafe = AnalyzeCertificate.IsReliableCertificate(certificateInfo, url);

                    SiteClassification.ModelInput sampleData = new SiteClassification.ModelInput()
                    {
                        ImageSource = imageByteArray,
                    };

                    // Load model and predict output
                    var result = SiteClassificationManager.Predict(sampleData);


                    if (result != null  )
                    {
 
                        if (UriComparison.IsSameOfficialSite(url, result.PredictedLabel)) {
                            // The website appears to be under surveillance and the URL is valid.
                            return Ok(true);
                        }
                        else {
                            // The website appears to be under surveillance, but the URL is different.
                            // Improve this system to consider subsidiaries as well...
                            return Ok(false);
                        }


                    }
                    else {
                        // The website does not resemble any monitored site, so it is considered safe.
                        return Ok(true);
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
