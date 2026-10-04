using Tesoreria.Domain.Entidades;

namespace Tesoreria.Application.Abstracciones.Persistencia;

public interface ICultoRepository
{
    Task<Culto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Culto>> ListarAsync(DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default);
    Task AgregarAsync(Culto culto, CancellationToken cancellationToken = default);
}
