using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProtocolAPI.Extensions
{
    public static class JwtServiceExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, string key)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;  // Certifique-se de que HTTPS está corretamente configurado em produção
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),  // Certifique-se de que a chave é a mesma usada para gerar o token
                    ValidateIssuer = false,  // Se estiver usando Issuer, defina aqui
                    ValidateAudience = false,  // Se estiver usando Audience, defina aqui
                    ValidateLifetime = true,  // Valida a expiração do token
                    ClockSkew = TimeSpan.Zero  // Para não permitir margem de erro de tempo na validação
                };
            });

        }
    }
}