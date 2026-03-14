using EmpreendedorismoSC.Application.Common;
using EmpreendedorismoSC.Application.DTOs;

namespace EmpreendedorismoSC.Application.Interfaces;

public interface IEmpreendimentoService
{
    Task<PagedResult<EmpreendimentoDto>> GetAllAsync(EmpreendimentoFilterDto filter);
    Task<EmpreendimentoDto?> GetByIdAsync(Guid id);
    Task<EmpreendimentoDto> CreateAsync(CreateEmpreendimentoDto dto);
    Task<EmpreendimentoDto?> UpdateAsync(Guid id, UpdateEmpreendimentoDto dto);
    Task<bool> DeleteAsync(Guid id);
}
