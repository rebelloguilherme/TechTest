using System.ComponentModel.DataAnnotations;
using EmpreendedorismoSC.Domain.Enums;

namespace EmpreendedorismoSC.Application.DTOs;

public class CreateEmpreendimentoDto
{
    [Required(ErrorMessage = "O nome do empreendimento é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O nome do empreendimento deve ter no máximo 200 caracteres.")]
    public string NomeEmpreendimento { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome do empreendedor é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O nome do empreendedor deve ter no máximo 200 caracteres.")]
    public string NomeEmpreendedor { get; set; } = string.Empty;

    [Required(ErrorMessage = "O município é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O município deve ter no máximo 100 caracteres.")]
    public string Municipio { get; set; } = string.Empty;

    [Required(ErrorMessage = "O segmento de atuação é obrigatório.")]
    public SegmentoAtuacao SegmentoAtuacao { get; set; }

    [Required(ErrorMessage = "O contato é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O contato deve ter no máximo 200 caracteres.")]
    public string Contato { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;
}
