namespace Tesoreria.Application.Abstracciones.Persistencia;

/// <summary>
/// Confirma en la base de datos los cambios hechos a través de los repositorios
/// dentro de un mismo Command. Lo implementa Infrastructure sobre el DbContext de EF Core.
/// </summary>
public interface IUnitOfWork
{
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
