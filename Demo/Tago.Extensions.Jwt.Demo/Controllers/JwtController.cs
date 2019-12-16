using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using Tago.Extensions.Jwt.Abstractions.Interfaces;
using Tago.Extensions.Jwt.Abstractions.Model;
using Tago.Extensions.Jwt.Mvc;

namespace Tago.Extensions.Jwt.Demo.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class JwtController : ControllerBase
    {
        private readonly ITokenGenerator tokenGenerator;
        private readonly ILogger<JwtController> logger;

        public JwtController(ITokenGenerator tokenGenerator, ILogger<JwtController> logger)
        {
            this.tokenGenerator = tokenGenerator;
            this.logger = logger;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Jwt demo api";
        }

        [HttpGet("test/default")]
        [Authorize]
        public ActionResult<string> TestDefault()
        {
            return "default test";
        }

        [HttpGet("test/policy")]
        [Authorize(policy: "Jwt1")]
        public ActionResult<string> TestPolicyBased()
        {
            return "policy test";
        }


        [HttpGet("generate")]
        public ActionResult<string> Generate(string kid)
        {
            try
            {
                this.logger.LogDebug($"generating test token kid: {kid}");

                //if kid null the token is unsigned

                JwtToken tknInfo = new JwtToken();
                tknInfo.Issuer = "me";
                tknInfo.Audience = "me";
                tknInfo.Claims.Add("test", "test");
                tknInfo.Claims.Add("firstName", "Avi");
                tknInfo.Claims.Add("profile", "admin");


                tknInfo.ValidFrom = DateTime.Now.AddMinutes(5);
                tknInfo.ValidTo = tknInfo.ValidFrom.Value.AddMinutes(2);

                var tkn = tokenGenerator.Generate(tknInfo, kid);
                return tkn;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //[HttpGet("generate/unsigned")]
        //public ActionResult<string> CreateUnsignedToken()
        //{
        //    try
        //    {
        //        return Generate(null);
        //    }
        //    catch(Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpGet("sign/{kid}")]
        public ActionResult<string> Sign([FromRoute]string kid, string token)
        {
            try
            {
                this.logger.LogDebug("create token test called..");
                var tkn = tokenGenerator.SignJwtToken(token, kid, validTo: DateTime.Now.AddMinutes(1));
                HttpContext.SetResponseAccessToken(tkn);
                return tkn;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("generate/{kid}")]
        public ActionResult<string> Sign([FromRoute]string kid, [FromBody] JwtToken request)
        {
            this.logger.LogDebug("create token test called..");
            var tkn = tokenGenerator.Generate(request, kid);
            HttpContext.SetResponseAccessToken(tkn);
            return tkn;
        }
    }

    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
