using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ExchangeRate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyForAuthenticationOfApplication"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var jwt = new JwtSecurityToken(
                //issuer: "User",
                //audience: "Dev",
                //expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: credentials);

            return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
        }
    }
}
