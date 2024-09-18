using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using ProtocolAPI.Data;
using ProtocolAPI.Models;

namespace ProtocolAPI.Repositories
{
    public class ProtocoloConsultaRepository
    {
        private readonly ProtocoloDbContext _context;

        public ProtocoloConsultaRepository(ProtocoloDbContext context)
        {
            _context = context;
        }

        // Consulta todos os protocolos com paginação
        public async Task<List<Protocolo>> ConsultarProtocolosPaginadosAsync(int page, int pageSize)
        {
            return await _context.Protocolos
                .Skip((page - 1) * pageSize)   // Pular os primeiros itens com base na página
                .Take(pageSize)                // Pegar o número de itens baseado no tamanho da página
                .ToListAsync();
        }

        // Retorna Total de Protocolos
        public async Task<int> ObterTotalProtocolosAsync()
        {
            return await _context.Protocolos.CountAsync();  // Contar o total de itens
        }

        // Consulta por Número de Protocolo
        public async Task<Protocolo> ConsultarPorNumeroProtocoloAsync(string numeroProtocolo)
        {
            return await _context.Protocolos
                .FirstOrDefaultAsync(p => p.NumeroProtocolo == numeroProtocolo);
        }

        // Consulta por CPF
        public async Task<IEnumerable<Protocolo>> ConsultarProtocolosPorCpfAsync(string cpf)
        {
            return await _context.Protocolos
                .Where(p => p.Cpf == cpf)
                .ToListAsync();
        }

        // Consulta por RG
        public async Task<IEnumerable<Protocolo>> ConsultarPorRgAsync(string rg)
        {
            return await _context.Protocolos
                .Where(p => p.Rg == rg)
                .ToListAsync();
        }
    }
}