using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Enums;

namespace Tesoreria.Application.Transacciones.Comandos.RegistrarEgreso;

public record RegistrarEgresoCommand(
    DateTime Fecha,
    int FondoId,
    decimal Monto,
    MedioPago MedioPago,
    int UsuarioRegistroId,
    string? Concepto,
    string? NumeroComprobante) : IRequest<TransaccionDto>;
