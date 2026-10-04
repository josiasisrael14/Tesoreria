using Microsoft.EntityFrameworkCore;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Infrastructure.Persistencia;

namespace Tesoreria.Infrastructure.Repositorios;

public class CultoRepository : ICultoRepository
{
    private readonly TesoreriaDbContext _dbContext;

    public CultoRepository(TesoreriaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Culto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _dbContext.Cultos.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<List<Culto>> ListarAsync(DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Cultos.AsNoTracking().AsQueryable();

        if (desde.HasValue)
            query = query.Where(c => c.Fecha >= desde.Value.Date);

        if (hasta.HasValue)
            query = query.Where(c => c.Fecha <= hasta.Value.Date);

        return await query.OrderByDescending(c => c.Fecha).ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(Culto culto, CancellationToken cancellationToken = default)
        => await _dbContext.Cultos.AddAsync(culto, cancellationToken);
}
