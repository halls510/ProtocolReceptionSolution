using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ProtocolConsumer.Models;
using ProtocolConsumer.Data;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Exceptions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Extensions.Hosting;

namespace ProtocolConsumer.Services
{
    public class ProtocolConsumerService
    {
        private readonly ProtocoloDbContext _context;
        private readonly IConnectionFactory _connectionFactory;
        private readonly string _queueName;

        // Construtor atualizado para permitir a injeção da fábrica de conexão e do nome da fila
        public ProtocolConsumerService(ProtocoloDbContext context, IConnectionFactory connectionFactory, string queueName)
        {
            _context = context;
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }

        public void StartConsuming()
        {
            int retryAttempts = 5;
            while (retryAttempts > 0)
            {
                try
                {
                    using var connection = _connectionFactory.CreateConnection();
                    using var channel = connection.CreateModel();

                    channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        HandleMessage(ea);
                    };

                    channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

                    Console.WriteLine("Aguardando mensagens...");

                    // Substitua o Console.ReadLine() por um loop infinito ou outro método de bloqueio
                    while (true)
                    {
                        Thread.Sleep(1000); // Aguarda sem bloquear a execução do consumidor
                    }
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

        // Método para processar a mensagem recebida
        public void HandleMessage(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var protocolo = JsonSerializer.Deserialize<Protocolo>(message);

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (ValidarProtocolo(protocolo))
                {
                    // Salvar protocolo válido
                    _context.Protocolos.Add(protocolo);
                    _context.SaveChanges();

                    transaction.Commit();
                    Console.WriteLine($"Protocolo armazenado: {protocolo.NumeroProtocolo}");
                    Log.Information($"Protocolo armazenado com sucesso: {protocolo.NumeroProtocolo}");
                }
                else
                {
                    Log.Error($"Erro: Protocolo inválido - {protocolo.NumeroProtocolo}");
                    Console.WriteLine($"Erro: Protocolo inválido - {protocolo.NumeroProtocolo}");

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                // Rollback da transação em caso de erro
                transaction.Rollback();
                Log.Error(ex, $"Erro ao armazenar o protocolo: {protocolo.NumeroProtocolo}");
                Console.WriteLine($"Erro na transação: {ex.Message}");
            }
        }

        private bool ValidarProtocolo(Protocolo protocolo)
        {
            // Validação 1: Número do protocolo obrigatório e único
            if (string.IsNullOrWhiteSpace(protocolo.NumeroProtocolo))
            {
                Log.Error($"Erro: Validar Protocolo. Número do protocolo é obrigatório. Numero Protocolo: {protocolo.NumeroProtocolo}");
                Console.WriteLine("Erro: Número do protocolo é obrigatório.");
                return false;
            }
            if (_context.Protocolos.Any(p => p.NumeroProtocolo == protocolo.NumeroProtocolo))
            {
                Log.Error($"Erro:  Validar Protocolo. Número do protocolo já existe. Numero Protocolo: {protocolo.NumeroProtocolo}");
                Console.WriteLine("Erro: Número do protocolo já existe.");
                return false;
            }

            // Validação 2: Número da via obrigatório
            if (protocolo.NumeroVia <= 0)
            {
                Log.Error($"Erro:  Validar Protocolo. Número da via é obrigatório e deve ser maior que zero. Numero da via: {protocolo.NumeroVia}");
                Console.WriteLine("Erro: Número da via é obrigatório e deve ser maior que zero.");
                return false;
            }

            // Validação 3: CPF obrigatório e único com o número da via
            if (string.IsNullOrWhiteSpace(protocolo.Cpf))
            {
                Log.Error($"Erro:  Validar Protocolo.  CPF é obrigatório. CPF: {protocolo.Cpf}");
                Console.WriteLine("Erro: CPF é obrigatório.");
                return false;
            }

            if (_context.Protocolos.Any(p => p.Cpf == protocolo.Cpf && p.NumeroVia == protocolo.NumeroVia))
            {
                Log.Error($"Erro:  Validar Protocolo.  Já existe uma via para o CPF com o mesmo número de via. CPF: {protocolo.Cpf} e Via: {protocolo.NumeroVia}");
                Console.WriteLine("Erro: Já existe uma via para o CPF com o mesmo número de via.");
                return false;
            }

            // Validação 4: RG obrigatório
            if (string.IsNullOrWhiteSpace(protocolo.Rg))
            {
                Log.Error($"Erro:  Validar Protocolo. RG é obrigatório. RG: {protocolo.Rg}");
                Console.WriteLine("Erro: RG é obrigatório.");
                return false;
            }

            // Verifica se o RG já tem uma via com o mesmo número
            if (_context.Protocolos.Any(p => p.Rg == protocolo.Rg && p.NumeroVia == protocolo.NumeroVia))
            {
                Log.Error($"Erro:  Validar Protocolo. Já existe uma via para o RG com o mesmo número de via. RG: {protocolo.Rg} e Via: {protocolo.NumeroVia}");
                Console.WriteLine("Erro: Já existe uma via para o RG com o mesmo número de via.");
                return false;
            }

            // Validação 5: Nome obrigatório
            if (string.IsNullOrWhiteSpace(protocolo.Nome))
            {
                Log.Error($"Erro:  Validar Protocolo. Nome é obrigatório. Nome: {protocolo.Nome}");
                Console.WriteLine("Erro: Nome é obrigatório.");
                return false;
            }

            // Validação 6: Foto obrigatória e no formato correto (jpg ou png)
            if (string.IsNullOrWhiteSpace(protocolo.Foto) || !ValidarFoto(protocolo.Foto))
            {
                Log.Error($"Erro:  Validar Protocolo.  Foto é obrigatória e deve estar no formato jpg ou png. Foto: {protocolo.Foto}");
                Console.WriteLine("Erro: Foto é obrigatória e deve estar no formato jpg ou png.");
                return false;
            }

            // Todas as validações passaram
            return true;
        }


        // Função para validar a foto
        private bool ValidarFoto(string caminhoFoto)
        {
            // Verificar se o caminho é nulo ou vazio
            if (string.IsNullOrEmpty(caminhoFoto)) return false;

            // Extrair a extensão do arquivo
            var extensao = Path.GetExtension(caminhoFoto).ToLower();

            // Verificar se a extensão é válida (.jpg ou .png)
            if (extensao == ".jpg" || extensao == ".png")
            {
                return true;  // A extensão é válida
            }

            // Extensão não permitida
            return false;
        }

    }
}