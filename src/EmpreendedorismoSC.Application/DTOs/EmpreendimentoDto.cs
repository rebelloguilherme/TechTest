using EmpreendedorismoSC.Domain.Enums;

namespace EmpreendedorismoSC.Application.DTOs;

public class EmpreendimentoDto
{
    public Guid Id { get; set; }
    public string NomeEmpreendimento { get; set; } = string.Empty;
    public string NomeEmpreendedor { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public SegmentoAtuacao SegmentoAtuacao { get; set; }
    public string SegmentoAtuacaoDescricao => SegmentoAtuacao.ToString();
    public string Contato { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
