using EmpreendedorismoSC.Domain.Entities;

namespace EmpreendedorismoSC.Domain.Interfaces;

public interface IEmpreendimentoRepository
{
    Task<IEnumerable<Empreendimento>> GetAllAsync();
    Task<Empreendimento?> GetByIdAsync(Guid id);
    Task<Empreendimento> AddAsync(Empreendimento empreendimento);
    Task<Empreendimento> UpdateAsync(Empreendimento empreendimento);
    Task<bool> DeleteAsync(Guid id);
}
