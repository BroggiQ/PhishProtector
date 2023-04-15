
using Microsoft.AspNetCore.Mvc;
using PhishAnalyzer;
using System.Globalization;

namespace PhishProtector.Windows.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AnalyzeController : ControllerBase
    {
        [HttpPost]
      //  public IActionResult Post([FromForm] string url, [FromForm] byte[] screenBytes)
        public IActionResult Post([FromForm] string url, [FromForm] string screenBytes)
        {
            try { 
            if(!string.IsNullOrEmpty(url)   && screenBytes != null) {
                    int base64StartIndex = screenBytes.IndexOf(',') + 1;
                    string base64String = screenBytes.Substring(base64StartIndex);

                    byte[] imageByteArray = Convert.FromBase64String(base64String);
 
 
                    SiteClassification.ModelInput sampleData = new SiteClassification.ModelInput()
                    {
                        ImageSource = imageByteArray,
                    };

                    // Load model and predict output
                    var result = SiteClassificationManager.Predict(sampleData);
                }
            return Ok();
            }
            catch (Exception ex)
            {
                return Ok();
            }
        }

    }
}
