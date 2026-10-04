using Microsoft.EntityFrameworkCore;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Infrastructure.Persistencia;

namespace Tesoreria.Infrastructure.Repositorios;

public class MiembroRepository : IMiembroRepository
{
    private readonly TesoreriaDbContext _dbContext;

    public MiembroRepository(TesoreriaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Miembro?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _dbContext.Miembros.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<List<Miembro>> ListarAsync(bool soloActivos, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Miembros.AsNoTracking().AsQueryable();

        if (soloActivos)
            query = query.Where(m => m.Activo);

        return await query.OrderBy(m => m.Apellidos).ThenBy(m => m.Nombres).ToListAsync(cancellationToken);
    }

    public async Task AgregarAsync(Miembro miembro, CancellationToken cancellationToken = default)
        => await _dbContext.Miembros.AddAsync(miembro, cancellationToken);

    public Task<int> ContarActivosAsync(CancellationToken cancellationToken = default)
        => _dbContext.Miembros.AsNoTracking().CountAsync(m => m.Activo, cancellationToken);

    public async Task<(List<Miembro> Items, int Total)> ListarPaginadoAsync(bool soloActivos, string? busqueda,
        int pagina, int tamanoPagina, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Miembros.AsNoTracking().AsQueryable();

        // A diferencia de ListarAsync (donde soloActivos=false significa "sin filtro, trae todos"),
        // acá el filtro es exclusivo: soloActivos=true trae solo los activos y soloActivos=false trae
        // solo los desactivados — así la casilla "Mostrar inactivos" de la pantalla de Miembros
        // muestra de verdad únicamente a los que están desactivados, no una mezcla de ambos.
        query = query.Where(m => m.Activo == soloActivos);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var texto = busqueda.Trim();
            query = query.Where(m =>
                EF.Functions.Like(m.Nombres, $"%{texto}%") ||
                EF.Functions.Like(m.Apellidos, $"%{texto}%") ||
                (m.DocumentoIdentidad != null && EF.Functions.Like(m.DocumentoIdentidad, $"%{texto}%")));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(m => m.Apellidos).ThenBy(m => m.Nombres)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
