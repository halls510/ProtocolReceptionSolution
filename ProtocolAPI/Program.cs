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


Log.Information("Iniciando aplica��o ProtocolAPI");

var builder = WebApplication.CreateBuilder(args);

// Configura��o do Entity Framework com SQL Server
// Configura��o do banco de dados usando vari�vel de ambiente
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

if (string.IsNullOrEmpty(connectionString))
{
    Log.Error("A vari�vel de ambiente DB_CONNECTION_STRING n�o est� definida.");
    throw new InvalidOperationException("A vari�vel de ambiente DB_CONNECTION_STRING n�o est� definida.");
}

// Configura��o do Entity Framework com SQLite persistente (somente leitura)
builder.Services.AddDbContext<ProtocoloDbContext>(options =>
   options.UseSqlite(connectionString));

// Configura��o do Swagger
builder.Services.AddSwaggerDocumentation();

// Configura��o da autentica��o JWT
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

if (string.IsNullOrEmpty(jwtSecretKey))
{
    Log.Error("A vari�vel de ambiente JWT_SECRET_KEY n�o est� definida.");

    throw new InvalidOperationException("A vari�vel de ambiente JWT_SECRET_KEY n�o est� definida.");
}
builder.Services.AddJwtAuthentication(jwtSecretKey);

// Registrar o reposit�rio e o servi�o
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

// Configurar autentica��o e autoriza��o
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores
app.MapControllers();

app.Run();

