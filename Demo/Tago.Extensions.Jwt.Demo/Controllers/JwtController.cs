using JwtWrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Tago.Extensions.Encryption;
using Tago.Extensions.Http;
using Tago.Extensions.Http.Interfaces;
using Tago.Extensions.Jwt.Abstractions.Model;
using Tago.Settings.Security;

namespace Tago.Infra.Web.Tester.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize(policy: "Jwt1")]
    public class JwtController : ControllerBase
    {
        private ILogger logger = null;
        private readonly ITokenSigner tokenSigner;

        public JwtController(ILogger<JwtController> logger, ITokenSigner tokenSigner)
        {
            this.logger = logger;
            this.tokenSigner = tokenSigner;
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



        [HttpGet("generate/test")]
        public string Generate()
        {
            JwtToken token = new JwtToken
            {
                Audience = "Me",
                Issuer = "Me",
                ValidTo = DateTime.Now.AddYears(1),
            };

            var dt = DateTime.Now;
            token.ValidFrom = dt;
            token.ValidTo = dt.AddDays(5);
            token.Claims.Add("amr", "pass");
            token.Claims.Add("amr", "pass_2");
            token.Claims.Add("sub", "psu-user-name");
            token.Claims.Add("MyName", "Golan Sheetrit");


            var s = JsonConvert.SerializeObject(token, Formatting.Indented);

            //return BadRequest

            return Generate(token);
        }


        [HttpPost("generate/jwt")]
        public string Generate(JwtToken token)
        {
            return tokenSigner.GenerateUnsigned(token);
        }

        [HttpPost("generate/payload")]
        public async Task<ActionResult<string>> GenerateFormPayload()
        {
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(body))
                {
                    var payload = JwtPayload.Deserialize(body);

                    return Ok(tokenSigner.GenerateUnsigned(payload));
                }
                else
                {
                    return BadRequest("invalid jwt payload");
                }
            }
        }


        [HttpGet("sign")]
        public ActionResult<string> SignString([FromQuery]string token, [FromQuery]string key)
        {
            try
            {
                var jwtToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ReadJwtToken(token);
                return this.tokenSigner.Sign(jwtToken, key ?? jwtToken.Issuer);
            }
            catch (Exception ex)
            {
                return BadRequest("invalid jwt string");
            }
        }

        [HttpPost("sign/jwt")]
        public string SignJwtObject(JwtToken token, [FromQuery]string key = null)
        {
            return tokenSigner.Sign(token, key ?? token?.Issuer);
        }

        [HttpPost("sign/payload")]
        public async Task<ActionResult<string>> SignJwtPayload([FromQuery]string key = null)
        {
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(body))
                {
                    var payload = JwtPayload.Deserialize(body);

                    return Ok(tokenSigner.Sign(payload, key ?? payload.Iss));
                }
                else
                {
                    return BadRequest("invalid jwt payload");
                }
            }
        }

        //[HttpGet("sign/{kid}")]
        //public ActionResult<string> Sign([FromRoute] string kid, [FromQuery]string token)
        //{
        //    try
        //    {
        //        var jwtToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().ReadJwtToken(token);
        //        return this.tokenSigner.Sign(jwtToken, kid);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("invalid jwt string");
        //    }
        //}


        //[HttpPost("sign")]
        //public string Sign([FromQuery]string key, JwtToken token)
        //{
        //    return tokenSigner.Sign(token, key ?? token?.Issuer);
        //}








    }
}
