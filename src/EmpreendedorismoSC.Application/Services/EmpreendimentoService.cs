using EmpreendedorismoSC.Application.Common;
using EmpreendedorismoSC.Application.DTOs;
using EmpreendedorismoSC.Application.Interfaces;
using EmpreendedorismoSC.Domain.Entities;
using EmpreendedorismoSC.Domain.Interfaces;
using FluentValidation;

namespace EmpreendedorismoSC.Application.Services;

public class EmpreendimentoService : IEmpreendimentoService
{
    private readonly IEmpreendimentoRepository _repository;
    private readonly IValidator<CreateEmpreendimentoDto> _createValidator;
    private readonly IValidator<UpdateEmpreendimentoDto> _updateValidator;

    public EmpreendimentoService(
        IEmpreendimentoRepository repository,
        IValidator<CreateEmpreendimentoDto> createValidator,
        IValidator<UpdateEmpreendimentoDto> updateValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<EmpreendimentoDto>> GetAllAsync(EmpreendimentoFilterDto filter)
    {
        // Validar limites de paginação
        if (filter.Pagina < 1) filter.Pagina = 1;
        if (filter.TamanhoPagina < 1) filter.TamanhoPagina = 10;
        if (filter.TamanhoPagina > 50) filter.TamanhoPagina = 50;

        var (items, totalCount) = await _repository.GetAllAsync(
            municipio: filter.Municipio,
            segmentoAtuacao: filter.SegmentoAtuacao,
            ativo: filter.Ativo,
            busca: filter.Busca,
            pagina: filter.Pagina,
            tamanhoPagina: filter.TamanhoPagina);

        return new PagedResult<EmpreendimentoDto>
        {
            Items = items.Select(MapToDto),
            TotalItems = totalCount,
            Pagina = filter.Pagina,
            TamanhoPagina = filter.TamanhoPagina
        };
    }

    public async Task<EmpreendimentoDto?> GetByIdAsync(Guid id)
    {
        var empreendimento = await _repository.GetByIdAsync(id);
        return empreendimento is null ? null : MapToDto(empreendimento);
    }

    public async Task<EmpreendimentoDto> CreateAsync(CreateEmpreendimentoDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errors);
        }

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
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException(errors);
        }

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
