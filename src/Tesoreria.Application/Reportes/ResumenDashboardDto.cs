using Tesoreria.Application.Transacciones;

namespace Tesoreria.Application.Reportes;

public record IngresoPorFondoResumen(string NombreFondo, decimal Total);

/// <summary>
/// Cuánto lleva recaudada una campaña de ofrenda especial activa. PorcentajeAvance
/// es null cuando la campaña no tiene meta definida (no se puede calcular progreso).
/// </summary>
public record ProgresoOfrendaEspecialDto(
    int OfrendaEspecialId,
    string Nombre,
    decimal Recaudado,
    decimal? Meta,
    double? PorcentajeAvance);

/// <summary>El resumen que alimenta el dashboard: la foto completa de cómo está la iglesia, de un vistazo.</summary>
public record ResumenDashboardDto(
    decimal TotalIngresosMes,
    decimal TotalEgresosMes,
    decimal SaldoNetoMes,
    /// <summary>Variación de ingresos vs. el mes anterior, en %. Null si el mes anterior no tuvo ingresos (no hay base para comparar).</summary>
    decimal? VariacionIngresosPorcentaje,
    List<IngresoPorFondoResumen> IngresosPorFondo,
    List<ProgresoOfrendaEspecialDto> CampanasActivas,
    int MiembrosActivos,
    List<TransaccionDto> UltimasTransacciones);
