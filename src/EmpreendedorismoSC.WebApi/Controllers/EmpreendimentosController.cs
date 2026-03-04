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
    /// Lista todos os empreendimentos cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EmpreendimentoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EmpreendimentoDto>>> GetAll()
    {
        var empreendimentos = await _service.GetAllAsync();
        return Ok(empreendimentos);
    }

    /// <summary>
    /// Busca um empreendimento pelo ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmpreendimentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmpreendimentoDto>> GetById(Guid id)
    {
        var empreendimento = await _service.GetByIdAsync(id);
        if (empreendimento is null)
            return NotFound();

        return Ok(empreendimento);
    }

    /// <summary>
    /// Cadastra um novo empreendimento.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmpreendimentoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmpreendimentoDto>> Create([FromBody] CreateEmpreendimentoDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza um empreendimento existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EmpreendimentoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmpreendimentoDto>> Update(Guid id, [FromBody] UpdateEmpreendimentoDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    /// <summary>
    /// Remove um empreendimento pelo ID.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
