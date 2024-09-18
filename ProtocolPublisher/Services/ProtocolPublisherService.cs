using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using ProtocolPublisher.Models;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace ProtocolPublisher.Services
{
    public class ProtocolPublisherService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _queueName;

        // Construtor atualizado para permitir a injeção da fábrica de conexão e do nome da fila
        public ProtocolPublisherService(IConnectionFactory connectionFactory, string queueName)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        // Método que publica os protocolos na fila do RabbitMQ
        public void PublishProtocols(List<Protocolo> protocolos)
        {
            int retryAttempts = 5;
            while (retryAttempts > 0)
            {
                try
                {
                    using var connection = _connectionFactory.CreateConnection();
                    using var channel = connection.CreateModel();

                    channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    foreach (var protocolo in protocolos)
                    {
                        string message = JsonSerializer.Serialize(protocolo);
                        var body = Encoding.UTF8.GetBytes(message);

                        try
                        {
                            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
                            Console.WriteLine($"[x] Protocolo Enviado: {protocolo.NumeroProtocolo}");
                            Log.Information($"Protocolo enviado com sucesso: {protocolo.NumeroProtocolo}");
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, $"Erro ao enviar o protocolo: {protocolo.NumeroProtocolo}");
                        }
                    }
                    break;  // Se conectar com sucesso, sair do loop
                }
                catch (BrokerUnreachableException ex)
                {
                    retryAttempts--;
                    Console.WriteLine($"Falha na conexão com RabbitMQ. Tentando novamente... {retryAttempts} tentativas restantes.");
                    Log.Error(ex, $"Falha na conexão com RabbitMQ. Tentando novamente... {retryAttempts} tentativas restantes.");
                    Thread.Sleep(5000);  // Aguardar 5 segundos antes de tentar novamente

                }
            }
        }
    }
}