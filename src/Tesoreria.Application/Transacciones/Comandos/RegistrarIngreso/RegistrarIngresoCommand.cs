using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Enums;

namespace Tesoreria.Application.Transacciones.Comandos.RegistrarIngreso;

/// <summary>
/// La pantalla más usada del sistema: registrar lo recogido en un culto
/// (diezmo, ofrenda general, ofrenda especial, etc.).
/// </summary>
public record RegistrarIngresoCommand(
    DateTime Fecha,
    int FondoId,
    decimal Monto,
    MedioPago MedioPago,
    int UsuarioRegistroId,
    int? MiembroId,
    int? CultoId,
    int? OfrendaEspecialId,
    string? Concepto,
    string? NumeroComprobante) : IRequest<TransaccionDto>;
