using EmpreendedorismoSC.Application.Common;
using EmpreendedorismoSC.Application.DTOs;
using EmpreendedorismoSC.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmpreendedorismoSC.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpreendimentosController : ControllerBase
{
    private readonly IEmpreendimentoService _service;

    public EmpreendimentosController(IEmpreendimentoService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista empreendimentos com filtros e paginação.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EmpreendimentoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] EmpreendimentoFilterDto filter)
    {
        var result = await _service.GetAllAsync(filter);
        return Ok(ApiResponse<PagedResult<EmpreendimentoDto>>.Ok(result, "Empreendimentos listados com sucesso."));
    }

    /// <summary>
    /// Busca um empreendimento pelo ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EmpreendimentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var empreendimento = await _service.GetByIdAsync(id);
        if (empreendimento is null)
            return NotFound(ApiResponse.Erro("Empreendimento não encontrado."));

        return Ok(ApiResponse<EmpreendimentoDto>.Ok(empreendimento));
    }

    /// <summary>
    /// Cadastra um novo empreendimento.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EmpreendimentoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEmpreendimentoDto dto)
    {
        var created = await _service.CreateAsync(dto);
        var response = ApiResponse<EmpreendimentoDto>.Ok(created, "Empreendimento criado com sucesso.");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
    }

    /// <summary>
    /// Atualiza um empreendimento existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EmpreendimentoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmpreendimentoDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(ApiResponse.Erro("Empreendimento não encontrado."));

        return Ok(ApiResponse<EmpreendimentoDto>.Ok(updated, "Empreendimento atualizado com sucesso."));
    }

    /// <summary>
    /// Remove um empreendimento pelo ID.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse.Erro("Empreendimento não encontrado."));

        return Ok(ApiResponse.Ok("Empreendimento removido com sucesso."));
    }
}
