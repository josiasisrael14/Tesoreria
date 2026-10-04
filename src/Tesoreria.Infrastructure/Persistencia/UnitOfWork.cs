using Tesoreria.Application.Abstracciones.Persistencia;

namespace Tesoreria.Infrastructure.Persistencia;

public class UnitOfWork : IUnitOfWork
{
    private readonly TesoreriaDbContext _dbContext;

    public UnitOfWork(TesoreriaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
