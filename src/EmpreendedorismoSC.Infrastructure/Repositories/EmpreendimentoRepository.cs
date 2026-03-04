using EmpreendedorismoSC.Domain.Entities;
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

    public async Task<IEnumerable<Empreendimento>> GetAllAsync()
    {
        return await _context.Empreendimentos
            .AsNoTracking()
            .OrderByDescending(e => e.DataCriacao)
            .ToListAsync();
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
