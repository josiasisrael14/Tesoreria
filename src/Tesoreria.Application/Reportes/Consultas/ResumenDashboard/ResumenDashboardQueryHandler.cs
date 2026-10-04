using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Application.Transacciones;

namespace Tesoreria.Application.Reportes.Consultas.ResumenDashboard;

public class ResumenDashboardQueryHandler : IRequestHandler<ResumenDashboardQuery, ResumenDashboardDto>
{
    private const int CantidadUltimasTransacciones = 8;

    private readonly ITransaccionRepository _transaccionRepository;
    private readonly IMiembroRepository _miembroRepository;
    private readonly IOfrendaEspecialRepository _ofrendaEspecialRepository;

    public ResumenDashboardQueryHandler(
        ITransaccionRepository transaccionRepository,
        IMiembroRepository miembroRepository,
        IOfrendaEspecialRepository ofrendaEspecialRepository)
    {
        _transaccionRepository = transaccionRepository;
        _miembroRepository = miembroRepository;
        _ofrendaEspecialRepository = ofrendaEspecialRepository;
    }

    public async Task<ResumenDashboardDto> Handle(ResumenDashboardQuery request, CancellationToken cancellationToken)
    {
        var hoy = DateTime.UtcNow.Date;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
        var inicioMesAnterior = inicioMes.AddMonths(-1);
        var finMesAnterior = inicioMes.AddDays(-1);

        var balanceMesActual = await _transaccionRepository.ObtenerBalancePorFondoAsync(inicioMes, hoy, cancellationToken);
        var balanceMesAnterior = await _transaccionRepository.ObtenerBalancePorFondoAsync(inicioMesAnterior, finMesAnterior, cancellationToken);

        var totalIngresosMes = balanceMesActual.Sum(b => b.TotalIngresos);
        var totalEgresosMes = balanceMesActual.Sum(b => b.TotalEgresos);
        var totalIngresosMesAnterior = balanceMesAnterior.Sum(b => b.TotalIngresos);

        decimal? variacionIngresos = totalIngresosMesAnterior > 0
            ? Math.Round((totalIngresosMes - totalIngresosMesAnterior) / totalIngresosMesAnterior * 100, 1)
            : null;

        var ingresosPorFondo = balanceMesActual
            .Where(b => b.TotalIngresos > 0)
            .OrderByDescending(b => b.TotalIngresos)
            .Select(b => new IngresoPorFondoResumen(b.NombreFondo, b.TotalIngresos))
            .ToList();

        var campanasActivas = await _ofrendaEspecialRepository.ListarAsync(soloActivos: true, cancellationToken);
        var totalesPorCampana = await _transaccionRepository.ObtenerTotalesPorOfrendaEspecialAsync(cancellationToken);
        var recaudadoPorId = totalesPorCampana.ToDictionary(t => t.OfrendaEspecialId, t => t.Total);

        var progresoCampanas = campanasActivas
            .Select(c =>
            {
                var recaudado = recaudadoPorId.TryGetValue(c.Id, out var total) ? total : 0m;
                double? porcentaje = c.MetaMonto is > 0
                    ? (double)Math.Min(recaudado / c.MetaMonto.Value, 1m) * 100
                    : null;

                return new ProgresoOfrendaEspecialDto(c.Id, c.Nombre, recaudado, c.MetaMonto, porcentaje);
            })
            .ToList();

        var miembrosActivos = await _miembroRepository.ContarActivosAsync(cancellationToken);

        var ultimasTransacciones = await _transaccionRepository.ObtenerUltimasAsync(CantidadUltimasTransacciones, cancellationToken);

        return new ResumenDashboardDto(
            totalIngresosMes,
            totalEgresosMes,
            totalIngresosMes - totalEgresosMes,
            variacionIngresos,
            ingresosPorFondo,
            progresoCampanas,
            miembrosActivos,
            ultimasTransacciones.Select(TransaccionDto.DesdeEntidad).ToList());
    }
}
