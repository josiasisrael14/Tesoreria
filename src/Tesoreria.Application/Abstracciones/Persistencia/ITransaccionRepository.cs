using Tesoreria.Domain.Entidades;
using Tesoreria.Domain.Enums;

namespace Tesoreria.Application.Abstracciones.Persistencia;

/// <summary>Fila agregada usada por el reporte de balance por fondo (BalancePorFondoQuery).</summary>
public record BalancePorFondoRegistro(int FondoId, string NombreFondo, decimal TotalIngresos, decimal TotalEgresos)
{
    public decimal Saldo => TotalIngresos - TotalEgresos;
}

/// <summary>Cuánto ha dado un miembro en un fondo (neto: ingresos menos reversas). Usado en su historial de aportes.</summary>
public record AporteFondoRegistro(int FondoId, string NombreFondo, decimal Total);

/// <summary>Cuánto se ha recaudado en una campaña de ofrenda especial (neto). Con MiembroId filtra a un solo miembro; sin filtro, es el total de la campaña (para el dashboard).</summary>
public record AporteOfrendaEspecialRegistro(int OfrendaEspecialId, string NombreOfrendaEspecial, decimal Total);

/// <summary>Cuánto ha dado cada miembro (neto) dentro de UNA campaña puntual. Para comparar contra los Compromisos.</summary>
public record AporteMiembroEnCampanaRegistro(int MiembroId, decimal Total);

public class FiltroTransacciones
{
    public int? FondoId { get; init; }
    public int? CultoId { get; init; }
    public int? MiembroId { get; init; }
    public int? OfrendaEspecialId { get; init; }
    public DateTime? Desde { get; init; }
    public DateTime? Hasta { get; init; }
    public MedioPago? MedioPago { get; init; }
}

public interface ITransaccionRepository
{
    Task<Transaccion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Transaccion>> ListarAsync(FiltroTransacciones filtro, CancellationToken cancellationToken = default);
    Task AgregarAsync(Transaccion transaccion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca una transacción prácticamente idéntica (mismo tipo de movimiento, fondo,
    /// monto, medio de pago, usuario y miembro) creada hace muy pocos segundos.
    /// Protección contra el doble envío accidental: si se perdió la respuesta del
    /// servidor (p. ej. se cortó internet justo después de guardar) y el usuario
    /// reintenta, esto encuentra la transacción que ya se guardó en vez de dejar
    /// que se cree una segunda idéntica.
    /// </summary>
    Task<Transaccion?> BuscarPosibleDuplicadoAsync(
        TipoMovimiento tipoMovimiento,
        int fondoId,
        decimal monto,
        MedioPago medioPago,
        int usuarioRegistroId,
        int? miembroId,
        DateTime creadaDespuesDe,
        CancellationToken cancellationToken = default);

    /// <summary>Las últimas N transacciones registradas (para el dashboard). Ordenadas por fecha, más reciente primero.</summary>
    Task<List<Transaccion>> ObtenerUltimasAsync(int cantidad, CancellationToken cancellationToken = default);

    /// <summary>
    /// Suma ingresos y egresos por fondo dentro de un rango de fechas. Como ya no usamos
    /// Dapper, esto se resuelve con un GroupBy de LINQ que EF Core traduce a SQL
    /// (un solo query a la base, no se trae todo a memoria).
    /// </summary>
    Task<List<BalancePorFondoRegistro>> ObtenerBalancePorFondoAsync(
        DateTime desde, DateTime hasta, CancellationToken cancellationToken = default);

    /// <summary>Cuánto ha dado un miembro, agrupado por fondo (para su historial de aportes: diezmos, ofrenda general, etc.).</summary>
    Task<List<AporteFondoRegistro>> ObtenerAportesPorFondoDeMiembroAsync(
        int miembroId, CancellationToken cancellationToken = default);

    /// <summary>Cuánto ha dado un miembro, agrupado por campaña de ofrenda especial.</summary>
    Task<List<AporteOfrendaEspecialRegistro>> ObtenerAportesPorOfrendaEspecialDeMiembroAsync(
        int miembroId, CancellationToken cancellationToken = default);

    /// <summary>Cuánto se ha recaudado en total (todos los miembros, histórico) en cada campaña de ofrenda especial. Para el progreso de campañas en el dashboard.</summary>
    Task<List<AporteOfrendaEspecialRegistro>> ObtenerTotalesPorOfrendaEspecialAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Cuánto ha dado cada miembro (neto) dentro de una campaña específica. Para comparar contra los Compromisos de esa campaña.</summary>
    Task<List<AporteMiembroEnCampanaRegistro>> ObtenerAportesPorMiembroEnCampanaAsync(
        int ofrendaEspecialId, CancellationToken cancellationToken = default);
}
