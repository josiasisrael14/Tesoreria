using Tesoreria.Domain.Entidades;

namespace Tesoreria.Application.Abstracciones.Persistencia;

public interface ICompromisoOfrendaRepository
{
    /// <summary>Incluye Miembro y OfrendaEspecial cargados, para no tener que pedirlos aparte.</summary>
    Task<CompromisoOfrenda?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);

    Task<CompromisoOfrenda?> ObtenerPorOfrendaYMiembroAsync(
        int ofrendaEspecialId, int miembroId, CancellationToken cancellationToken = default);

    /// <summary>Todos los compromisos de una campaña, con el miembro ya cargado.</summary>
    Task<List<CompromisoOfrenda>> ListarPorOfrendaEspecialAsync(
        int ofrendaEspecialId, CancellationToken cancellationToken = default);

    Task AgregarAsync(CompromisoOfrenda compromiso, CancellationToken cancellationToken = default);

    void Eliminar(CompromisoOfrenda compromiso);
}
