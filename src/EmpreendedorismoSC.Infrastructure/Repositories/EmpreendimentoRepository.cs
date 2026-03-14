using EmpreendedorismoSC.Domain.Entities;
using EmpreendedorismoSC.Domain.Enums;
using EmpreendedorismoSC.Domain.Interfaces;
using EmpreendedorismoSC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmpreendedorismoSC.Infrastructure.Repositories;

public class EmpreendimentoRepository : IEmpreendimentoRepository
{
    private readonly ApplicationDbContext _context;

    public EmpreendimentoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Empreendimento> Items, int TotalCount)> GetAllAsync(
        string? municipio = null,
        SegmentoAtuacao? segmentoAtuacao = null,
        bool? ativo = null,
        string? busca = null,
        int pagina = 1,
        int tamanhoPagina = 10)
    {
        var query = _context.Empreendimentos.AsNoTracking().AsQueryable();

        // Filtro por município (busca parcial, case-insensitive)
        if (!string.IsNullOrWhiteSpace(municipio))
            query = query.Where(e => e.Municipio.ToLower().Contains(municipio.ToLower()));

        // Filtro por segmento de atuação
        if (segmentoAtuacao.HasValue)
            query = query.Where(e => e.SegmentoAtuacao == segmentoAtuacao.Value);

        // Filtro por status
        if (ativo.HasValue)
            query = query.Where(e => e.Ativo == ativo.Value);

        // Busca textual no nome do empreendimento ou empreendedor
        if (!string.IsNullOrWhiteSpace(busca))
        {
            var buscaLower = busca.ToLower();
            query = query.Where(e =>
                e.NomeEmpreendimento.ToLower().Contains(buscaLower) ||
                e.NomeEmpreendedor.ToLower().Contains(buscaLower));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(e => e.DataCriacao)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Empreendimento?> GetByIdAsync(Guid id)
    {
        return await _context.Empreendimentos.FindAsync(id);
    }

    public async Task<Empreendimento> AddAsync(Empreendimento empreendimento)
    {
        _context.Empreendimentos.Add(empreendimento);
        await _context.SaveChangesAsync();
        return empreendimento;
    }

    public async Task<Empreendimento> UpdateAsync(Empreendimento empreendimento)
    {
        _context.Empreendimentos.Update(empreendimento);
        await _context.SaveChangesAsync();
        return empreendimento;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var empreendimento = await _context.Empreendimentos.FindAsync(id);
        if (empreendimento is null)
            return false;

        _context.Empreendimentos.Remove(empreendimento);
        await _context.SaveChangesAsync();
        return true;
    }
}
