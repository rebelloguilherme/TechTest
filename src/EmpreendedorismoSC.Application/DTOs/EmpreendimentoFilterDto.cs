using EmpreendedorismoSC.Domain.Enums;

namespace EmpreendedorismoSC.Application.DTOs;

public class EmpreendimentoFilterDto
{
    /// <summary>
    /// Filtrar por município (busca parcial, case-insensitive).
    /// </summary>
    public string? Municipio { get; set; }

    /// <summary>
    /// Filtrar por segmento de atuação.
    /// </summary>
    public SegmentoAtuacao? SegmentoAtuacao { get; set; }

    /// <summary>
    /// Filtrar por status (ativo/inativo).
    /// </summary>
    public bool? Ativo { get; set; }

    /// <summary>
    /// Busca textual livre (pesquisa nos campos: Nome do Empreendimento e Nome do Empreendedor). Case-insensitive, busca parcial.
    /// </summary>
    public string? Busca { get; set; }

    /// <summary>
    /// Número da página (padrão: 1).
    /// </summary>
    public int Pagina { get; set; } = 1;

    /// <summary>
    /// Quantidade de itens por página (padrão: 10, máximo: 50).
    /// </summary>
    public int TamanhoPagina { get; set; } = 10;
}
