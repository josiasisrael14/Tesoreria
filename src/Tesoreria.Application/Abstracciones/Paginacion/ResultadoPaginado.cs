namespace Tesoreria.Application.Abstracciones.Paginacion;

/// <summary>
/// Página de resultados para listados que pueden crecer mucho (ej. miembros).
/// </summary>
public record ResultadoPaginado<T>(List<T> Items, int TotalRegistros, int Pagina, int TamanoPagina)
{
    public int TotalPaginas => TamanoPagina <= 0
        ? 0
        : (int)Math.Ceiling(TotalRegistros / (double)TamanoPagina);
}
