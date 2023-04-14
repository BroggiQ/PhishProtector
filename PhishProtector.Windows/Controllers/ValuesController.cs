using System;
using System.Windows.Forms;
using Microsoft.AspNetCore.Mvc;

namespace PhishProtector.Windows.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        //Form1 form1 = new Form1();
        //[HttpGet]
        //public ActionResult<string> Get()
        //{
        //    string text = "";
        //    form1.Invoke(new Action(() =>
        //    {
        //        text = form1.textBox1.Text;
        //    }));
        //    return text;
        //}

        //[HttpGet("{id}")]
        //public ActionResult Get(string id)
        //{
        //    form1.Invoke(new Action(() =>
        //    {
        //        form1.UpdateText(id);
        //    }));
        //    return Ok();
        //}
    }
}
