using Microsoft.AspNetCore.Mvc;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Reportes.Consultas.ResumenDashboard;
using Tesoreria.Application.Transacciones.Consultas.BalancePorFondo;

namespace Tesoreria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Cuánto entró y salió por fondo en un rango de fechas (el reporte principal de tesorería).</summary>
    [HttpGet("balance-por-fondo")]
    public async Task<IActionResult> BalancePorFondo(
        [FromQuery] DateTime desde, [FromQuery] DateTime hasta, CancellationToken cancellationToken)
    {
        var balance = await _mediator.Send(new BalancePorFondoQuery(desde, hasta), cancellationToken);
        return Ok(balance);
    }

    /// <summary>La portada del sistema: KPIs del mes, ingresos por fondo, progreso de campañas y últimas transacciones.</summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
    {
        var resumen = await _mediator.Send(new ResumenDashboardQuery(), cancellationToken);
        return Ok(resumen);
    }
}
