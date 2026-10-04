using Microsoft.EntityFrameworkCore;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Infrastructure.Persistencia;

namespace Tesoreria.Infrastructure.Repositorios;

public class OfrendaEspecialRepository : IOfrendaEspecialRepository
{
    private readonly TesoreriaDbContext _dbContext;

    public OfrendaEspecialRepository(TesoreriaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<OfrendaEspecial?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _dbContext.OfrendasEspeciales.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public async Task<List<OfrendaEspecial>> ListarAsync(bool soloActivos, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.OfrendasEspeciales.AsNoTracking().AsQueryable();

        if (soloActivos)
            query = query.Where(o => o.Activo);

        return await query.OrderByDescending(o => o.Anio).ThenBy(o => o.Nombre).ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(OfrendaEspecial ofrendaEspecial, CancellationToken cancellationToken = default)
        => await _dbContext.OfrendasEspeciales.AddAsync(ofrendaEspecial, cancellationToken);

    public Task<bool> ExisteConNombreYAnioAsync(string nombre, int anio, CancellationToken cancellationToken = default)
        => _dbContext.OfrendasEspeciales.AnyAsync(o => o.Nombre == nombre && o.Anio == anio, cancellationToken);
}
