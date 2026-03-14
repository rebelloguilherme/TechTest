using EmpreendedorismoSC.Domain.Entities;
using EmpreendedorismoSC.Domain.Enums;

namespace EmpreendedorismoSC.Domain.Interfaces;

public interface IEmpreendimentoRepository
{
    Task<(IEnumerable<Empreendimento> Items, int TotalCount)> GetAllAsync(
        string? municipio = null,
        SegmentoAtuacao? segmentoAtuacao = null,
        bool? ativo = null,
        string? busca = null,
        int pagina = 1,
        int tamanhoPagina = 10);

    Task<Empreendimento?> GetByIdAsync(Guid id);
    Task<Empreendimento> AddAsync(Empreendimento empreendimento);
    Task<Empreendimento> UpdateAsync(Empreendimento empreendimento);
    Task<bool> DeleteAsync(Guid id);
}
