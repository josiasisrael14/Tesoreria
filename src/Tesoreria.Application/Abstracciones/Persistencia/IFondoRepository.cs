using Tesoreria.Domain.Entidades;

namespace Tesoreria.Application.Abstracciones.Persistencia;

public interface IFondoRepository
{
    Task<Fondo?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Fondo>> ListarAsync(bool soloActivos, CancellationToken cancellationToken = default);
    Task AgregarAsync(Fondo fondo, CancellationToken cancellationToken = default);
    Task<bool> ExisteConNombreAsync(string nombre, CancellationToken cancellationToken = default);

    /// <summary>El único fondo marcado como "el" fondo de Ofrendas Especiales, si ya hay uno.</summary>
    Task<Fondo?> ObtenerFondoDeOfrendasEspecialesAsync(CancellationToken cancellationToken = default);
}
