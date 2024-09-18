using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtocolAPI.Services;
using Serilog;

namespace ProtocolAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProtocolosController : ControllerBase
    {
        private readonly ProtocoloConsultaService _consultaService;

        public ProtocolosController(ProtocoloConsultaService consultaService)
        {
            _consultaService = consultaService;
        }

        /// <summary>
        /// Retorna uma lista de protocolos paginados.
        /// </summary>
        /// <remarks>
        /// Esse endpoint permite consultar todos os protocolos de forma paginada, 
        /// permitindo especificar a página e o tamanho da página para controlar a quantidade de dados retornados.
        /// </remarks>
        /// <param name="page">Número da página a ser retornada (padrão: 1).</param>
        /// <param name="pageSize">Quantidade de itens por página (padrão: 10).</param>
        /// <returns>Retorna um objeto contendo a lista de protocolos, o total de itens, o número da página e o tamanho da página.</returns>
        /// <response code="200">Lista de protocolos retornada com sucesso.</response>
        /// <response code="404">Nenhum protocolo encontrado.</response>
        [HttpGet("paginados")]
        public async Task<IActionResult> ConsultarTodosPaginados([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // Obter protocolos paginados
            var protocolos = await _consultaService.ConsultarProtocolosPaginadosAsync(page, pageSize);
            var total = await _consultaService.ObterTotalProtocolosAsync();

            if (protocolos == null || !protocolos.Any())
            {
                Log.Warning("PNenhum protocolo encontrado.");
                return NotFound("Nenhum protocolo encontrado.");
            }

            // Criar um objeto para a resposta de paginação
            var response = new
            {
                TotalItems = total,
                Page = page,
                PageSize = pageSize,
                Items = protocolos
            };

            return Ok(response);
        }

        /// <summary>
        /// Consulta um protocolo específico por número de protocolo.
        /// </summary>
        /// <remarks>
        /// O número de protocolo deve ser fornecido no formato "00001", "00002", etc.
        /// 
        /// Exemplos de números de protocolos que podem ser consultados:
        /// - 00001
        /// - 00002
        /// - 00003
        /// - 00004
        /// - 00005
        /// - 00006
        /// - 00007
        /// - 00008
        /// - 00009
        /// - 00010
        /// </remarks>
        /// <param name="numeroProtocolo">Número do protocolo (ex: "00001").</param>
        /// <returns>Retorna os detalhes do protocolo associado ao número informado.</returns>
        /// <response code="200">Protocolo encontrado e retornado com sucesso.</response>
        /// <response code="404">Nenhum protocolo foi encontrado com o número fornecido.</response>
        [HttpGet("numero/{numeroProtocolo}")]
        public async Task<IActionResult> ConsultarPorNumeroProtocolo(string numeroProtocolo)
        {
            var protocolo = await _consultaService.ConsultarPorNumeroProtocoloAsync(numeroProtocolo);

            if (protocolo == null)
            {
                Log.Warning($"Protocolo não encontrado: {numeroProtocolo}");
                return NotFound("Nenhum protocolo encontrado com o número de protocolo fornecido.");
            }

            Log.Information($"Consulta realizada com sucesso para o número de protocolo: {numeroProtocolo}");

            return Ok(protocolo);

        }

        /// <summary>
        /// Consulta uma lista de protocolos associados a um CPF.
        /// </summary>
        /// <remarks>
        /// O CPF deve ser fornecido com 11 dígitos, sem formatação.
        /// 
        /// Exemplos de CPFs que podem ser consultados:
        /// - 12345678901
        /// - 98765432100
        /// - 45678912345
        /// - 65432109876
        /// - 56789023456
        /// - 78901234567
        /// - 89012345678
        /// - 90123456789
        /// - 01234567890
        /// - 23456789012
        /// </remarks>
        /// <param name="cpf">CPF do titular dos protocolos (11 dígitos, sem formatação).</param>
        /// <returns>Retorna uma lista de protocolos associados ao CPF fornecido.</returns>
        /// <response code="200">Protocolos encontrados e retornados com sucesso.</response>
        /// <response code="404">Nenhum protocolo foi encontrado para o CPF fornecido.</response>>
        [HttpGet("cpf/{cpf}")]
        public async Task<IActionResult> ConsultarPorCpf(string cpf)
        {
            var protocolos = await _consultaService.ConsultarProtocolosPorCpfAsync(cpf);

            if (protocolos == null || !protocolos.Any())
            {
                Log.Warning($"Nenhum protocolo encontrado para o CPF: {cpf}");
                return NotFound("Nenhum protocolo encontrado para o CPF fornecido.");
            }

            Log.Information($"Consulta realizada com sucesso para o cpf: {cpf}");
            return Ok(protocolos);
        }

        /// <summary>
        /// Consulta uma lista de protocolos associados a um RG.
        /// </summary>
        /// <remarks>
        /// O RG deve ser fornecido no formato "SP100001", "MG100002", etc.
        /// 
        /// Exemplos de RGs que podem ser consultados:
        /// - SP100001
        /// - MG100002
        /// - RJ100003
        /// - ES100004
        /// - DF100005
        /// - BA100006
        /// - SC100007
        /// - GO100008
        /// - PR100009
        /// - CE100010
        /// </remarks>
        /// <param name="rg">RG do titular dos protocolos (ex: "SP100001").</param>
        /// <returns>Retorna uma lista de protocolos associados ao RG fornecido.</returns>
        /// <response code="200">Protocolos encontrados e retornados com sucesso.</response>
        /// <response code="404">Nenhum protocolo foi encontrado para o RG fornecido.</response>
        [HttpGet("rg/{rg}")]
        public async Task<IActionResult> ConsultarPorRg(string rg)
        {
            var protocolos = await _consultaService.ConsultarPorRgAsync(rg);

            if (protocolos == null || !protocolos.Any())
            {
                Log.Warning($"Nenhum protocolo encontrado para o RG: {rg}");
                return NotFound("Nenhum protocolo encontrado para o RG fornecido.");
            }

            Log.Information($"Consulta realizada com sucesso para o RG: {rg}");
            return Ok(protocolos);
        }
    }
}