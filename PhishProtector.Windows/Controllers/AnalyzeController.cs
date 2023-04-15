
using Microsoft.AspNetCore.Mvc;
using PhishAnalyzer;
using System.Globalization;
using System.Security.Policy;

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
            try
            {
                if (!string.IsNullOrEmpty(url) && screenBytes != null)
                {
                    int base64StartIndex = screenBytes.IndexOf(',') + 1;
                    string base64String = screenBytes.Substring(base64StartIndex);

                    byte[] imageByteArray = Convert.FromBase64String(base64String);


                    SiteClassification.ModelInput sampleData = new SiteClassification.ModelInput()
                    {
                        ImageSource = imageByteArray,
                    };

                    // Load model and predict output
                    var result = SiteClassificationManager.Predict(sampleData);


                    if (result != null  )
                    {
 
                        if (UriComparaison.IsSameOfficialSite(url, result.PredictedLabel)) {
                            //Le site  ressemble à un site sous surveillance et l'url est bonne
                            return Ok(true);
                        }
                        else {
                            //Le site  ressemble à un site sous surveillance mais l'url n'est pas la même
                            //Ameliorer ce système pour prendre en compte les filliales....
                            return Ok(false);
                        }


                    }
                    else { 
                        //Le site ne ressemble à aucun site sous surveillance donc c'est bon
                        return Ok(true);
                    }
                }
                //L'url ou le screen du site est vie impossible de traiter le site
                return BadRequest("Url and/or Screen are empty");
            }
            catch (Exception ex)
            {
                //Erreur interne
                return StatusCode(500, ex.Message); 
             }
        }

    }
}
