using Microsoft.EntityFrameworkCore;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Domain.Enums;
using Tesoreria.Infrastructure.Persistencia;

namespace Tesoreria.Infrastructure.Repositorios;

public class TransaccionRepository : ITransaccionRepository
{
    private readonly TesoreriaDbContext _dbContext;

    public TransaccionRepository(TesoreriaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Transaccion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _dbContext.Transacciones
            .Include(t => t.Fondo)
            .Include(t => t.Miembro)
            .Include(t => t.OfrendaEspecial)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<List<Transaccion>> ListarAsync(FiltroTransacciones filtro, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Transacciones
            .AsNoTracking()
            .Include(t => t.Fondo)
            .Include(t => t.Miembro)
            .Include(t => t.OfrendaEspecial)
            .AsQueryable();

        if (filtro.FondoId.HasValue)
            query = query.Where(t => t.FondoId == filtro.FondoId.Value);

        if (filtro.CultoId.HasValue)
            query = query.Where(t => t.CultoId == filtro.CultoId.Value);

        if (filtro.MiembroId.HasValue)
            query = query.Where(t => t.MiembroId == filtro.MiembroId.Value);

        if (filtro.OfrendaEspecialId.HasValue)
            query = query.Where(t => t.OfrendaEspecialId == filtro.OfrendaEspecialId.Value);

        if (filtro.Desde.HasValue)
            query = query.Where(t => t.Fecha >= filtro.Desde.Value.Date);

        if (filtro.Hasta.HasValue)
            query = query.Where(t => t.Fecha <= filtro.Hasta.Value.Date);

        if (filtro.MedioPago.HasValue)
            query = query.Where(t => t.MedioPago == filtro.MedioPago.Value);

        return await query.OrderByDescending(t => t.Fecha).ThenByDescending(t => t.Id).ToListAsync(cancellationToken);
    }

    public async Task<List<Transaccion>> ObtenerUltimasAsync(int cantidad, CancellationToken cancellationToken = default)
        => await _dbContext.Transacciones
            .AsNoTracking()
            .Include(t => t.Fondo)
            .Include(t => t.Miembro)
            .Include(t => t.OfrendaEspecial)
            .OrderByDescending(t => t.Fecha)
            .ThenByDescending(t => t.Id)
            .Take(cantidad)
            .ToListAsync(cancellationToken);

    public async Task AgregarAsync(Transaccion transaccion, CancellationToken cancellationToken = default)
        => await _dbContext.Transacciones.AddAsync(transaccion, cancellationToken);

    public Task<Transaccion?> BuscarPosibleDuplicadoAsync(
        TipoMovimiento tipoMovimiento,
        int fondoId,
        decimal monto,
        MedioPago medioPago,
        int usuarioRegistroId,
        int? miembroId,
        DateTime creadaDespuesDe,
        CancellationToken cancellationToken = default)
        => _dbContext.Transacciones
            .AsNoTracking()
            .Include(t => t.Fondo)
            .Include(t => t.Miembro)
            .Include(t => t.OfrendaEspecial)
            .Where(t => !t.EsReversa
                && t.TipoMovimiento == tipoMovimiento
                && t.FondoId == fondoId
                && t.Monto == monto
                && t.MedioPago == medioPago
                && t.UsuarioRegistroId == usuarioRegistroId
                && t.MiembroId == miembroId
                && t.FechaCreacion >= creadaDespuesDe)
            .OrderByDescending(t => t.FechaCreacion)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<BalancePorFondoRegistro>> ObtenerBalancePorFondoAsync(
        DateTime desde, DateTime hasta, CancellationToken cancellationToken = default)
    {
        // GroupBy de LINQ -> EF Core lo traduce a un GROUP BY con SUM(CASE WHEN ...) en SQL:
        // un solo viaje a la base de datos, nada de traer todo a memoria y sumar en C#.
        var query = _dbContext.Transacciones
            .AsNoTracking()
            .Where(t => t.Fecha >= desde.Date && t.Fecha <= hasta.Date)
            .GroupBy(t => new { t.FondoId, t.Fondo.Nombre })
            .Select(g => new
            {
                g.Key.FondoId,
                g.Key.Nombre,
                TotalIngresos = g.Where(x => x.TipoMovimiento == TipoMovimiento.Ingreso).Sum(x => x.Monto),
                TotalEgresos = g.Where(x => x.TipoMovimiento == TipoMovimiento.Egreso).Sum(x => x.Monto)
            });

        var resultado = await query.ToListAsync(cancellationToken);

        return resultado
            .Select(r => new BalancePorFondoRegistro(r.FondoId, r.Nombre, r.TotalIngresos, r.TotalEgresos))
            .OrderBy(r => r.NombreFondo)
            .ToList();
    }

    public async Task<List<AporteFondoRegistro>> ObtenerAportesPorFondoDeMiembroAsync(
        int miembroId, CancellationToken cancellationToken = default)
    {
        // Neto = ingresos - egresos dentro del mismo fondo. Un egreso con MiembroId solo
        // existe como reversa de un ingreso de ese miembro, así que esto descuenta
        // automáticamente cualquier aporte que haya sido corregido/reversado.
        var query = _dbContext.Transacciones
            .AsNoTracking()
            .Where(t => t.MiembroId == miembroId)
            .GroupBy(t => new { t.FondoId, t.Fondo.Nombre })
            .Select(g => new
            {
                g.Key.FondoId,
                g.Key.Nombre,
                Ingresos = g.Where(x => x.TipoMovimiento == TipoMovimiento.Ingreso).Sum(x => x.Monto),
                Egresos = g.Where(x => x.TipoMovimiento == TipoMovimiento.Egreso).Sum(x => x.Monto)
            });

        var resultado = await query.ToListAsync(cancellationToken);

        return resultado
            .Select(r => new AporteFondoRegistro(r.FondoId, r.Nombre, r.Ingresos - r.Egresos))
            .Where(r => r.Total != 0)
            .ToList();
    }

    public async Task<List<AporteOfrendaEspecialRegistro>> ObtenerAportesPorOfrendaEspecialDeMiembroAsync(
        int miembroId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Transacciones
            .AsNoTracking()
            .Where(t => t.MiembroId == miembroId && t.OfrendaEspecialId != null)
            .GroupBy(t => new { t.OfrendaEspecialId, t.OfrendaEspecial!.Nombre })
            .Select(g => new
            {
                g.Key.OfrendaEspecialId,
                g.Key.Nombre,
                Ingresos = g.Where(x => x.TipoMovimiento == TipoMovimiento.Ingreso).Sum(x => x.Monto),
                Egresos = g.Where(x => x.TipoMovimiento == TipoMovimiento.Egreso).Sum(x => x.Monto)
            });

        var resultado = await query.ToListAsync(cancellationToken);

        return resultado
            .Select(r => new AporteOfrendaEspecialRegistro(r.OfrendaEspecialId!.Value, r.Nombre, r.Ingresos - r.Egresos))
            .Where(r => r.Total != 0)
            .ToList();
    }

    public async Task<List<AporteOfrendaEspecialRegistro>> ObtenerTotalesPorOfrendaEspecialAsync(
        CancellationToken cancellationToken = default)
    {
        // Igual que el histórico por miembro, pero sin filtrar por miembro: cuánto
        // lleva recaudado cada campaña en total, para la barra de progreso del dashboard.
        var query = _dbContext.Transacciones
            .AsNoTracking()
            .Where(t => t.OfrendaEspecialId != null)
            .GroupBy(t => new { t.OfrendaEspecialId, t.OfrendaEspecial!.Nombre })
            .Select(g => new
            {
                g.Key.OfrendaEspecialId,
                g.Key.Nombre,
                Ingresos = g.Where(x => x.TipoMovimiento == TipoMovimiento.Ingreso).Sum(x => x.Monto),
                Egresos = g.Where(x => x.TipoMovimiento == TipoMovimiento.Egreso).Sum(x => x.Monto)
            });

        var resultado = await query.ToListAsync(cancellationToken);

        return resultado
            .Select(r => new AporteOfrendaEspecialRegistro(r.OfrendaEspecialId!.Value, r.Nombre, r.Ingresos - r.Egresos))
            .ToList();
    }

    public async Task<List<AporteMiembroEnCampanaRegistro>> ObtenerAportesPorMiembroEnCampanaAsync(
        int ofrendaEspecialId, CancellationToken cancellationToken = default)
    {
        // Igual patrón que los demás aportes (neto = ingresos - egresos/reversas),
        // pero agrupado por miembro dentro de UNA sola campaña — es lo que se
        // compara contra CompromisoOfrenda.MontoComprometido.
        var query = _dbContext.Transacciones
            .AsNoTracking()
            .Where(t => t.OfrendaEspecialId == ofrendaEspecialId && t.MiembroId != null)
            .GroupBy(t => t.MiembroId!.Value)
            .Select(g => new
            {
                MiembroId = g.Key,
                Ingresos = g.Where(x => x.TipoMovimiento == TipoMovimiento.Ingreso).Sum(x => x.Monto),
                Egresos = g.Where(x => x.TipoMovimiento == TipoMovimiento.Egreso).Sum(x => x.Monto)
            });

        var resultado = await query.ToListAsync(cancellationToken);

        return resultado
            .Select(r => new AporteMiembroEnCampanaRegistro(r.MiembroId, r.Ingresos - r.Egresos))
            .ToList();
    }
}
