using Tesoreria.Domain.Entidades;

namespace Tesoreria.Application.Abstracciones.Persistencia;

public interface IMiembroRepository
{
    Task<Miembro?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Miembro>> ListarAsync(bool soloActivos, CancellationToken cancellationToken = default);
    Task AgregarAsync(Miembro miembro, CancellationToken cancellationToken = default);

    /// <summary>Cuántos miembros activos hay (para el dashboard) — un COUNT, no trae la lista completa.</summary>
    Task<int> ContarActivosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Listado paginado y con búsqueda por nombre/apellido/documento, para que la
    /// pantalla de miembros no cargue todos los registros cuando hay muchos.
    /// </summary>
    Task<(List<Miembro> Items, int Total)> ListarPaginadoAsync(bool soloActivos, string? busqueda,
        int pagina, int tamanoPagina, CancellationToken cancellationToken = default);
}
