using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using ResiGrass_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ResiGrass_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        // Este endpoint se usa para autenticar y obtener un token
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserCredentials credentials)
        {
            // Validar las credenciales con valores hardcodeados
            if (IsValidUser(credentials.Username, credentials.Password))
            {
                var token = GenerateJwtToken(credentials.Username);
                return Ok(new { Token = token });
            }

            return Unauthorized(); // Si las credenciales son incorrectas
        }

        // Método para validar las credenciales (valores hardcodeados)
        private bool IsValidUser(string username, string password)
        {
            return username == "resigrass" && password == ",BuVMAo109{J";
        }

        // Método para generar el token JWT
        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("G8j!q%Z7pT@x$3Rw^eY&f2*BnL9k#4Hs"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "api.santiago.com",
                audience: "api.resigrass.com",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}
