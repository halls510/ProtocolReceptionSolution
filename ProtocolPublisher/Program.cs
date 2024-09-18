using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtocolPublisher.Mocks;
using ProtocolPublisher.Services;
using RabbitMQ.Client;
using Serilog;

// Configurar Serilog para salvar os logs na pasta de logs
var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/app_log.txt")
    .CreateLogger();

Log.Logger = logger;


Log.Information("Iniciando aplicação ProtocolPublisher");

var host = Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration((context, config) =>
{
    config.SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddEnvironmentVariables();
})
.ConfigureServices((context, services) =>
{
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

    // Registrar o ProtocolPublisherService com o nome da fila
    services.AddTransient<ProtocolPublisherService>(sp =>
    {
        var connectionFactory = sp.GetRequiredService<IConnectionFactory>();
        var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");

        if (string.IsNullOrEmpty(queueName))
        {
            Log.Error("A variável de ambiente QUEUE_NAME não está definida.");

            throw new InvalidOperationException("A variável de ambiente QUEUE_NAME não está definida.");
        }

        return new ProtocolPublisherService(connectionFactory, queueName);
    });
})
.Build();

// Iniciar o serviço de publicação
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var publisherService = services.GetRequiredService<ProtocolPublisherService>();

    // Mockar e publicar os protocolos
    var protocolos = MockarProtocolos.GerarMockDeProtocolos();
    publisherService.PublishProtocols(protocolos);
}

await host.RunAsync();
