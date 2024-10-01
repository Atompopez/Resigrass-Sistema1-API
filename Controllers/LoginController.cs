using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private IConfiguration _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

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
            var jwt = _configuration.GetSection("Jwt").Get<JWT>();

            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new Claim("Username", username)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: singIn
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}