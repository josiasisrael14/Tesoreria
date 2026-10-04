using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Reportes.Consultas.ResumenDashboard;

/// <summary>La portada del sistema: KPIs del mes, ingresos por fondo, progreso de campañas y últimas transacciones.</summary>
public record ResumenDashboardQuery : IRequest<ResumenDashboardDto>;
