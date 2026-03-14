namespace EmpreendedorismoSC.Application.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalItems { get; set; }
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalItems / TamanhoPagina);
    public bool TemProximaPagina => Pagina < TotalPaginas;
    public bool TemPaginaAnterior => Pagina > 1;
}
