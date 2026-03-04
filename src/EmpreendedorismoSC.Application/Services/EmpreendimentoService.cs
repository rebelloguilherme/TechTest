using EmpreendedorismoSC.Application.DTOs;
using EmpreendedorismoSC.Application.Interfaces;
using EmpreendedorismoSC.Domain.Entities;
using EmpreendedorismoSC.Domain.Interfaces;

namespace EmpreendedorismoSC.Application.Services;

public class EmpreendimentoService : IEmpreendimentoService
{
    private readonly IEmpreendimentoRepository _repository;

    public EmpreendimentoService(IEmpreendimentoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EmpreendimentoDto>> GetAllAsync()
    {
        var empreendimentos = await _repository.GetAllAsync();
        return empreendimentos.Select(MapToDto);
    }

    public async Task<EmpreendimentoDto?> GetByIdAsync(Guid id)
    {
        var empreendimento = await _repository.GetByIdAsync(id);
        return empreendimento is null ? null : MapToDto(empreendimento);
    }

    public async Task<EmpreendimentoDto> CreateAsync(CreateEmpreendimentoDto dto)
    {
        var empreendimento = new Empreendimento
        {
            Id = Guid.NewGuid(),
            NomeEmpreendimento = dto.NomeEmpreendimento,
            NomeEmpreendedor = dto.NomeEmpreendedor,
            Municipio = dto.Municipio,
            SegmentoAtuacao = dto.SegmentoAtuacao,
            Contato = dto.Contato,
            Ativo = dto.Ativo,
            DataCriacao = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(empreendimento);
        return MapToDto(created);
    }

    public async Task<EmpreendimentoDto?> UpdateAsync(Guid id, UpdateEmpreendimentoDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return null;

        existing.NomeEmpreendimento = dto.NomeEmpreendimento;
        existing.NomeEmpreendedor = dto.NomeEmpreendedor;
        existing.Municipio = dto.Municipio;
        existing.SegmentoAtuacao = dto.SegmentoAtuacao;
        existing.Contato = dto.Contato;
        existing.Ativo = dto.Ativo;
        existing.DataAtualizacao = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existing);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    private static EmpreendimentoDto MapToDto(Empreendimento entity)
    {
        return new EmpreendimentoDto
        {
            Id = entity.Id,
            NomeEmpreendimento = entity.NomeEmpreendimento,
            NomeEmpreendedor = entity.NomeEmpreendedor,
            Municipio = entity.Municipio,
            SegmentoAtuacao = entity.SegmentoAtuacao,
            Contato = entity.Contato,
            Ativo = entity.Ativo,
            DataCriacao = entity.DataCriacao,
            DataAtualizacao = entity.DataAtualizacao
        };
    }
}
