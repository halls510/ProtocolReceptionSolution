using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtocolConsumer.Data;
using ProtocolConsumer.Services;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Serilog;

// Configurar Serilog para salvar os logs na pasta de logs
var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/app_log.txt")
    .CreateLogger();

Log.Logger = logger;


Log.Information("Iniciando aplicação ProtocolConsumer");

var host = Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration((context, config) =>
{
    config.SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddEnvironmentVariables();
})
.ConfigureServices((context, services) =>
{
    // Configuração do banco de dados usando variável de ambiente
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

    if (string.IsNullOrEmpty(connectionString))
    {
        Log.Error("A variável de ambiente DB_CONNECTION_STRING não está definida.");
        throw new InvalidOperationException("A variável de ambiente DB_CONNECTION_STRING não está definida.");
    }

    // Configuração do Entity Framework com SQLite persistente (somente leitura)
    services.AddDbContext<ProtocoloDbContext>(options =>
       options.UseSqlite(connectionString));

    // Registrar o IConnectionFactory no contêiner de DI
    services.AddSingleton<IConnectionFactory>(sp =>
    {
        // Criação da ConnectionFactory usando variáveis de ambiente
        var rabbitMqHostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME");
        var rabbitMqUserName = Environment.GetEnvironmentVariable("RABBITMQ_USERNAME");
        var rabbitMqPassword = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD");

        // Verificando se as variáveis de ambiente estão definidas
        if (string.IsNullOrEmpty(rabbitMqHostName))
        {
            Log.Error("A variável de ambiente RABBITMQ_HOSTNAME não está definida.");

            throw new InvalidOperationException("A variável de ambiente RABBITMQ_HOSTNAME não está definida.");
        }

        if (string.IsNullOrEmpty(rabbitMqUserName))
        {
            Log.Error("A variável de ambiente RABBITMQ_USER não está definida.");

            throw new InvalidOperationException("A variável de ambiente RABBITMQ_USER não está definida.");
        }

        if (string.IsNullOrEmpty(rabbitMqPassword))
        {
            Log.Error("A variável de ambiente RABBITMQ_PASSWORD não está definida.");

            throw new InvalidOperationException("A variável de ambiente RABBITMQ_PASSWORD não está definida.");
        }

        return new ConnectionFactory
        {
            HostName = rabbitMqHostName,
            UserName = rabbitMqUserName,
            Password = rabbitMqPassword
        };
    });

    // Adicionando o serviço responsável por consumir as mensagens do RabbitMQ
    services.AddTransient<ProtocolConsumerService>(sp =>
    {
        var context = sp.GetRequiredService<ProtocoloDbContext>();
        var connectionFactory = sp.GetRequiredService<IConnectionFactory>();
        var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");

        if (string.IsNullOrEmpty(queueName))
        {
            Log.Error("A variável de ambiente QUEUE_NAME não está definida.");

            throw new InvalidOperationException("A variável de ambiente QUEUE_NAME não está definida.");
        }

        return new ProtocolConsumerService(context, connectionFactory, queueName);
    });

})
.Build();

// Iniciar o serviço de consumo
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = scope.ServiceProvider.GetRequiredService<ProtocoloDbContext>();
    dbContext.Database.EnsureCreated();  // Garantir que o esquema do banco seja criado

    var consumerService = services.GetRequiredService<ProtocolConsumerService>();

    // Inicia o consumo das mensagens da fila RabbitMQ
    consumerService.StartConsuming();
}

await host.RunAsync();