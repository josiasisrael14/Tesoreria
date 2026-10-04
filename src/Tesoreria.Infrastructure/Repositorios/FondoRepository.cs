using Microsoft.EntityFrameworkCore;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Infrastructure.Persistencia;

namespace Tesoreria.Infrastructure.Repositorios;

public class FondoRepository : IFondoRepository
{
    private readonly TesoreriaDbContext _dbContext;

    public FondoRepository(TesoreriaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Fondo?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _dbContext.Fondos.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);

    public async Task<List<Fondo>> ListarAsync(bool soloActivos, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Fondos.AsNoTracking().AsQueryable();

        if (soloActivos)
            query = query.Where(f => f.Activo);

        return await query.OrderBy(f => f.Nombre).ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(Fondo fondo, CancellationToken cancellationToken = default)
        => await _dbContext.Fondos.AddAsync(fondo, cancellationToken);

    public Task<bool> ExisteConNombreAsync(string nombre, CancellationToken cancellationToken = default)
        => _dbContext.Fondos.AnyAsync(f => f.Nombre == nombre, cancellationToken);

    // Sin AsNoTracking a propósito: el Handler que llama esto normalmente necesita
    // desmarcarlo (mutar la entidad) y guardar el cambio en el mismo SaveChanges.
    public Task<Fondo?> ObtenerFondoDeOfrendasEspecialesAsync(CancellationToken cancellationToken = default)
        => _dbContext.Fondos.FirstOrDefaultAsync(f => f.EsFondoDeOfrendasEspeciales, cancellationToken);
}
