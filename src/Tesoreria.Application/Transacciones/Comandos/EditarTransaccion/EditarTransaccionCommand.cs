using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Enums;

namespace Tesoreria.Application.Transacciones.Comandos.EditarTransaccion;

/// <summary>
/// Corrige los datos de una transacción ya guardada (se escribió mal el monto,
/// el fondo, el miembro, etc.). A diferencia de Reversar, esto SÍ cambia el
/// registro en el sitio — pensado para el caso más común: un error de tipeo.
/// </summary>
public record EditarTransaccionCommand(
    int TransaccionId,
    DateTime Fecha,
    int FondoId,
    decimal Monto,
    MedioPago MedioPago,
    int? MiembroId,
    int? CultoId,
    int? OfrendaEspecialId,
    string? Concepto,
    string? NumeroComprobante) : IRequest<TransaccionDto>;
