//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System.IO;
//using System.Threading.Tasks;
//using Tago.Extensions.JwkUtils;
//using Tago.Extensions.Jwt.Demo.Model;
//using Tago.Extensions.Jwt.Handlers;

//namespace Tago.Infra.Web.Tester.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    //[Authorize(policy: "Jwt1")]
//    public class JwksController : ControllerBase
//    {
//        private readonly ILogger logger = null;
//        private readonly JwksSettings options;
//        public JwksController(ILogger<JwksController> logger, IOptionsMonitor<JwksSettings> options)
//        {
//            this.options = options.CurrentValue;
//            this.logger = logger;
//        }

//        [HttpGet("pem")]
//        [AllowAnonymous]
//        public async Task<ActionResult<string>> GetJwk([FromQuery] string kid)
//        {
//            using var rdr = new StreamReader(Request.Body);
//            string text = await rdr.ReadToEndAsync();
//            var obj = SecurityKeyProviderEx.GetPublicJwkString(text, kid);

//            if (obj != null)
//            {
//                return new ContentResult() { Content = obj, ContentType = "application/json", StatusCode = 200 };
//            }
//            else
//            {
//                return BadRequest();
//            }

//            //var str = System.IO.File.ReadAllText("T:\\Projects\\TAGO_TFS\\TAGO\\TAGO\\.NetCore\\Infra\\Infra-2.2\\Security\\Tago.Extensions.Jwt\\Jwks\\Test\\.well-known\\jwks.json");
//            //return Ok(str);
//        }

//        [HttpGet]
//        [AllowAnonymous]
//        public async Task<ActionResult<string>> Get(System.Threading.CancellationToken cancellationToken)
//        {
//            if (!string.IsNullOrWhiteSpace(options?.FilePath))
//            {
//                var str = await System.IO.File.ReadAllTextAsync(options?.FilePath, cancellationToken);
//                return new ContentResult() { Content = str, ContentType = "application/json", StatusCode = 200 };
//            }
//            else
//            {
//                return BadRequest("Jwks file not defined");
//            }
//        }
//    }
//}
