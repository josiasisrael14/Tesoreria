using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Enums;

namespace Tesoreria.Application.Transacciones.Consultas.ListarTransacciones;

public record ListarTransaccionesQuery(
    int? FondoId = null,
    int? CultoId = null,
    int? MiembroId = null,
    int? OfrendaEspecialId = null,
    DateTime? Desde = null,
    DateTime? Hasta = null,
    MedioPago? MedioPago = null) : IRequest<List<TransaccionDto>>;
