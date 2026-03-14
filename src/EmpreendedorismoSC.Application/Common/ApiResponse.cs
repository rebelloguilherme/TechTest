using System.Text.Json.Serialization;

namespace EmpreendedorismoSC.Application.Common;

public class ApiResponse<T>
{
    public bool Sucesso { get; set; }
    public string? Mensagem { get; set; }
    public T? Dados { get; set; }
    public IEnumerable<string>? Erros { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Metadata { get; set; }

    public static ApiResponse<T> Ok(T dados, string? mensagem = null)
    {
        return new ApiResponse<T>
        {
            Sucesso = true,
            Mensagem = mensagem,
            Dados = dados
        };
    }

    public static ApiResponse<T> OkPaged(PagedResult<T> paged, string? mensagem = null)
    {
        return new ApiResponse<T>
        {
            Sucesso = true,
            Mensagem = mensagem,
            Dados = default,
            Metadata = new
            {
                paged.TotalItems,
                paged.Pagina,
                paged.TamanhoPagina,
                paged.TotalPaginas,
                paged.TemProximaPagina,
                paged.TemPaginaAnterior
            }
        };
    }

    public static ApiResponse<T> Erro(string mensagem, IEnumerable<string>? erros = null)
    {
        return new ApiResponse<T>
        {
            Sucesso = false,
            Mensagem = mensagem,
            Erros = erros
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string? mensagem = null)
    {
        return new ApiResponse
        {
            Sucesso = true,
            Mensagem = mensagem
        };
    }

    public new static ApiResponse Erro(string mensagem, IEnumerable<string>? erros = null)
    {
        return new ApiResponse
        {
            Sucesso = false,
            Mensagem = mensagem,
            Erros = erros
        };
    }
}
