using Microsoft.EntityFrameworkCore;
using ProtocolAPI.Data;
using ProtocolAPI.Extensions;
using ProtocolAPI.Repositories;
using ProtocolAPI.Services;
using Serilog;

// Configurar Serilog para salvar os logs na pasta de logs
var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/app_log.txt")
    .CreateLogger();

Log.Logger = logger;


Log.Information("Iniciando aplicação ProtocolAPI");

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework com SQL Server
// Configuração do banco de dados usando variável de ambiente
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    Log.Error("A variável de ambiente DB_CONNECTION_STRING não está definida.");
    throw new InvalidOperationException("A variável de ambiente DB_CONNECTION_STRING não está definida.");
}

// Configuração do Entity Framework com SQLite persistente (somente leitura)
builder.Services.AddDbContext<ProtocoloDbContext>(options =>
   options.UseSqlite(connectionString));

// Configuração do Swagger
builder.Services.AddSwaggerDocumentation();

// Configuração da autenticação JWT
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

if (string.IsNullOrEmpty(jwtSecretKey))
{
    Log.Error("A variável de ambiente JWT_SECRET_KEY não está definida.");

    throw new InvalidOperationException("A variável de ambiente JWT_SECRET_KEY não está definida.");
}
builder.Services.AddJwtAuthentication(jwtSecretKey);

// Registrar o repositório e o serviço
builder.Services.AddScoped<ProtocoloConsultaRepository>();
builder.Services.AddScoped<ProtocoloConsultaService>();

// Adicionando controladores
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Protocol API v1");
});

// Configurar autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores
app.MapControllers();

app.Run();

