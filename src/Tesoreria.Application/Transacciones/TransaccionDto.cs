using Tesoreria.Domain.Entidades;
using Tesoreria.Domain.Enums;

namespace Tesoreria.Application.Transacciones;

public record TransaccionDto(
    int Id,
    DateTime Fecha,
    TipoMovimiento TipoMovimiento,
    decimal Monto,
    MedioPago MedioPago,
    string? Concepto,
    string? NumeroComprobante,
    int FondoId,
    string NombreFondo,
    int? MiembroId,
    string? NombreMiembro,
    int? CultoId,
    int? OfrendaEspecialId,
    string? NombreOfrendaEspecial,
    bool EsReversa,
    bool Reversada,
    // True si esta transacción NO es nueva: ya existía una prácticamente idéntica
    // creada hace pocos segundos y se devolvió esa en vez de crear una duplicada
    // (ver RegistrarIngresoCommandHandler / RegistrarEgresoCommandHandler). El
    // frontend puede usar esto para avisarle al usuario que no se duplicó nada.
    bool EsDuplicadoDetectado = false)
{
    /// <summary>
    /// Arma el DTO a partir de la entidad. Requiere que el repositorio haya
    /// cargado (Include) la navegación a Fondo, Miembro y OfrendaEspecial (si aplica).
    /// </summary>
    public static TransaccionDto DesdeEntidad(Transaccion t) => new(
        t.Id, t.Fecha, t.TipoMovimiento, t.Monto, t.MedioPago, t.Concepto, t.NumeroComprobante,
        t.FondoId, t.Fondo.Nombre, t.MiembroId, t.Miembro?.NombreCompleto, t.CultoId,
        t.OfrendaEspecialId, t.OfrendaEspecial?.Nombre, t.EsReversa, t.Reversada);
}
