using Microsoft.EntityFrameworkCore;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Infrastructure.Persistencia;

namespace Tesoreria.Infrastructure.Repositorios;

public class CompromisoOfrendaRepository : ICompromisoOfrendaRepository
{
    private readonly TesoreriaDbContext _dbContext;

    public CompromisoOfrendaRepository(TesoreriaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<CompromisoOfrenda?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _dbContext.CompromisosOfrenda
            .Include(c => c.Miembro)
            .Include(c => c.OfrendaEspecial)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<CompromisoOfrenda?> ObtenerPorOfrendaYMiembroAsync(
        int ofrendaEspecialId, int miembroId, CancellationToken cancellationToken = default)
        => _dbContext.CompromisosOfrenda
            .FirstOrDefaultAsync(c => c.OfrendaEspecialId == ofrendaEspecialId && c.MiembroId == miembroId, cancellationToken);

    public async Task<List<CompromisoOfrenda>> ListarPorOfrendaEspecialAsync(
        int ofrendaEspecialId, CancellationToken cancellationToken = default)
        => await _dbContext.CompromisosOfrenda
            .AsNoTracking()
            .Include(c => c.Miembro)
            .Where(c => c.OfrendaEspecialId == ofrendaEspecialId)
            .ToListAsync(cancellationToken);

    public async Task AgregarAsync(CompromisoOfrenda compromiso, CancellationToken cancellationToken = default)
        => await _dbContext.CompromisosOfrenda.AddAsync(compromiso, cancellationToken);

    public void Eliminar(CompromisoOfrenda compromiso)
        => _dbContext.CompromisosOfrenda.Remove(compromiso);
}
