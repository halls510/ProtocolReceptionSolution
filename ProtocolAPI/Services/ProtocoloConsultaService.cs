using ProtocolAPI.Models;
using ProtocolAPI.Repositories;

namespace ProtocolAPI.Services
{
    public class ProtocoloConsultaService
    {
        private readonly ProtocoloConsultaRepository _repository;

        public ProtocoloConsultaService(ProtocoloConsultaRepository repository)
        {
            _repository = repository;
        }

        // Método para consultar todos os protocolos com paginação
        public async Task<List<Protocolo>> ConsultarProtocolosPaginadosAsync(int page, int pageSize)
        {
            return await _repository.ConsultarProtocolosPaginadosAsync(page, pageSize);
        }

        // Método que retorna Total de Protocolos
        public async Task<int> ObterTotalProtocolosAsync()
        {
            return await _repository.ObterTotalProtocolosAsync();
        }

        // Método para consultar por Número de Protocolo
        public async Task<Protocolo> ConsultarPorNumeroProtocoloAsync(string numeroProtocolo)
        {
            return await _repository.ConsultarPorNumeroProtocoloAsync(numeroProtocolo);
        }

        // Método para consultar por CPF
        public async Task<IEnumerable<Protocolo>> ConsultarProtocolosPorCpfAsync(string cpf)
        {
            return await _repository.ConsultarProtocolosPorCpfAsync(cpf);
        }

        // Método para consultar por RG
        public async Task<IEnumerable<Protocolo>> ConsultarPorRgAsync(string rg)
        {
            return await _repository.ConsultarPorRgAsync(rg);
        }
    }
}