using Tesoreria.Domain.Entidades;

namespace Tesoreria.Application.Abstracciones.Persistencia;

public interface IOfrendaEspecialRepository
{
    Task<OfrendaEspecial?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<OfrendaEspecial>> ListarAsync(bool soloActivos, CancellationToken cancellationToken = default);
    Task AgregarAsync(OfrendaEspecial ofrendaEspecial, CancellationToken cancellationToken = default);
    Task<bool> ExisteConNombreYAnioAsync(string nombre, int anio, CancellationToken cancellationToken = default);
}
