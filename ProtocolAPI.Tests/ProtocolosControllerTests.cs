using Moq;
using Microsoft.AspNetCore.Mvc;
using ProtocolAPI.Controllers;
using ProtocolAPI.Services;
using ProtocolAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using ProtocolAPI.Repositories;
using ProtocolAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ProtocolAPI.Tests
{
    public class ProtocolosControllerTests : IAsyncLifetime
    {
        private readonly ProtocoloDbContext _dbContext;
        private readonly ProtocoloConsultaRepository _repository;
        private readonly ProtocoloConsultaService _consultaService;
        private readonly ProtocolosController _controller;

        public ProtocolosControllerTests()
        {
            // Configurando o banco de dados em memória para uso nos testes
            var options = new DbContextOptionsBuilder<ProtocoloDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ProtocoloDbContext(options);
            _repository = new ProtocoloConsultaRepository(_dbContext);
            _consultaService = new ProtocoloConsultaService(_repository);
            _controller = new ProtocolosController(_consultaService);
    
        }

        // Método assíncrono que será chamado antes dos testes para inicializar o banco de dados
        public async Task InitializeAsync()
        {
            await PreencherBancoDeDadosAsync();
        }

        // Não é necessário fazer nada no DisposeAsync, então retornamos uma tarefa completada
        public Task DisposeAsync() => Task.CompletedTask;

        private async Task PreencherBancoDeDadosAsync()
        {
            // Limpar o banco de dados antes de adicionar novos dados
            _dbContext.Protocolos.RemoveRange(_dbContext.Protocolos);
            await _dbContext.SaveChangesAsync();

            // Adicionar os novos dados ao banco
            _dbContext.Protocolos.AddRange(new List<Protocolo>
            {
                new Protocolo
                {
                    NumeroProtocolo = "00001",
                    NumeroVia = 1,
                    Cpf = "12345678901",
                    Rg = "SP100001",
                    Nome = "João Silva",
                    NomeMae = "Maria Silva",
                    NomePai = "José Silva",
                    Foto = "foto1.jpg"
                },
                new Protocolo
                {
                    NumeroProtocolo = "00002",
                    NumeroVia = 2,
                    Cpf = "12345678901",
                    Rg = "SP100002",
                    Nome = "Maria Silva",
                    NomeMae = "Lúcia Souza",
                    NomePai = "Pedro Souza",
                    Foto = "foto2.jpg"
                },
                new Protocolo
                {
                   NumeroProtocolo = "00003",
                   NumeroVia = 3,
                   Cpf = "55678912345",
                   Rg = "SP100003",
                   Nome = "Ana Souza",
                   NomeMae = "Lúcia Souza",
                   NomePai = "Pedro Souza",
                   Foto = "foto2.jpg"
                },
                 new Protocolo
                {
                   NumeroProtocolo = "00004",
                   NumeroVia = 4,
                   Cpf = "55678912345",
                   Rg = "SP100003",
                   Nome = "Ana Souza",
                   NomeMae = "Lúcia Souza",
                   NomePai = "Pedro Souza",
                   Foto = "foto2.jpg"
                }
            });

            await _dbContext.SaveChangesAsync();
        }


        #region ConsultarPorNumeroProtocolo Tests

        [Fact]
        public async Task ConsultarPorNumeroProtocolo_ProtocoloExistente_DeveRetornar200Ok()
        {

            // Act
            var result = await _controller.ConsultarPorNumeroProtocolo("00001") as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task ConsultarPorNumeroProtocolo_ProtocoloNaoExistente_DeveRetornar404NotFound()
        {
            // Act
            var result = await _controller.ConsultarPorNumeroProtocolo("99999") as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        #endregion

        #region ConsultarPorCpf Tests

        [Fact]
        public async Task ConsultarPorCpf_ProtocolosExistentes_DeveRetornar200Ok()
        {
            // Act
            var result = await _controller.ConsultarPorCpf("12345678901") as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            // Verificar o valor retornado
            var protocolos = Assert.IsType<List<Protocolo>>(result.Value);

            Assert.Equal(2, protocolos.Count); // Verifica se retornou os dois protocolos com o mesmo CPF

            // Verifica os detalhes dos protocolos retornados
            Assert.Contains(protocolos, p => p.NumeroProtocolo == "00001");
            Assert.Contains(protocolos, p => p.NumeroProtocolo == "00002");
        }

        [Fact]
        public async Task ConsultarPorCpf_NenhumProtocolo_DeveRetornar404NotFound()
        {
            // Act
            var result = await _controller.ConsultarPorCpf("98765432100") as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Nenhum protocolo encontrado para o CPF fornecido.", result.Value);
        }

        #endregion

        #region ConsultarPorRg Tests

        [Fact]
        public async Task ConsultarPorRg_ProtocolosExistentes_DeveRetornar200Ok()
        {
            // Arrange
            await PreencherBancoDeDadosAsync();

            // Act
            var result = await _controller.ConsultarPorRg("SP100003") as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            // Verifica se o resultado é uma lista de protocolos
            var protocolos = Assert.IsType<List<Protocolo>>(result.Value);

            // Verifica se há dois protocolos associados ao RG "SP100003"
            Assert.Equal(2, protocolos.Count);
            Assert.All(protocolos, p => Assert.Equal("SP100003", p.Rg)); // Verifica se todos os protocolos têm o RG correto
        }

        [Fact]
        public async Task ConsultarPorRg_NenhumProtocolo_DeveRetornar404NotFound()
        {
            // Arrange
            await PreencherBancoDeDadosAsync();  // Banco é preenchido, mas sem o RG "RJ100002"

            // Act
            var result = await _controller.ConsultarPorRg("RJ100002") as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Nenhum protocolo encontrado para o RG fornecido.", result.Value);
        }

        #endregion

    }
}