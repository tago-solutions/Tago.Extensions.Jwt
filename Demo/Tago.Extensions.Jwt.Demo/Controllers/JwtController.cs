using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Tago.Infra.Web.Tester.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize(policy: "Jwt1")]
    public class JwtController : ControllerBase
    {
        private ILogger logger = null;

        public JwtController(ILogger<JwtController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<string> Get()
        {
            return $"Welcome to jwt";
        }

        [HttpGet("test")]
        [Authorize]
        public ActionResult<string> TestDefault()
        {
            return "Welcome to jwt tester";
        }

        [HttpGet("test/{type}")]
        [Authorize]
        public ActionResult<string> TestDefault(string type)
        {
            return $"Welcome to jwt '{type}' tester";
        }


        [HttpGet("policy")]
        [Authorize(policy: "Jwt1")]
        public ActionResult<string> TestPolicy()
        {
            return $"Welcome to jwt 'Jwt1' policy tester";
        }



        //[HttpGet("generate/test")]
        //public string Generate()
        //{
        //    JwtToken token = new JwtToken
        //    {
        //        Audience = "Me",
        //        Issuer = "Me",
        //        ValidTo = DateTime.Now.AddYears(1),
        //    };

        //    var dt = DateTime.Now;
        //    token.ValidFrom = dt;
        //    token.ValidTo = dt.AddDays(5);
        //    token.Claims.Add("amr", "pass");
        //    token.Claims.Add("amr", "pass_2");
        //    token.Claims.Add("sub", "psu-user-name");
        //    token.Claims.Add("MyName", "Golan Sheetrit");


        //    var s = JsonConvert.SerializeObject(token, Formatting.Indented);

        //    //return BadRequest

        //    return Generate(token);
        //}


        //[HttpPost("generate/jwt")]
        //public string Generate(JwtToken token)
        //{
        //    return tokenSigner.GenerateUnsigned(token);
        //}

       
    }
}
