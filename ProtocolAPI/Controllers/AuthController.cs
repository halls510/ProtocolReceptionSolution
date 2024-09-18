using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProtocolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Endpoint para gerar token JWT
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            // Ler credenciais do appsettings.json ou variáveis de ambiente
            string validUsername = _configuration["Credentials:Username"];
            string validPassword = _configuration["Credentials:Password"];

            // Log de tentativa de login
            Log.Information("Tentativa de login para o usuário: {Username}", login.Username);


            // Validação simples de usuário e senha
            if (login.Username != validUsername || login.Password != validPassword)
            {
                Log.Warning("Tentativa de login inválida para o usuário: {Username}", login.Username);
                return Unauthorized(new { message = "Usuário ou senha inválidos" });
            }

            // Configuração da autenticação JWT
            var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

            if (string.IsNullOrEmpty(jwtSecretKey))
            {
                Log.Error("Variável de ambiente JWT_SECRET_KEY não está definida.");
                throw new InvalidOperationException("A variável de ambiente JWT_SECRET_KEY não está definida.");
            }

            // Geração do token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecretKey); 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, login.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}