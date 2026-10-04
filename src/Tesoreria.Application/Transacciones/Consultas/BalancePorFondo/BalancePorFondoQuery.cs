using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;

namespace Tesoreria.Application.Transacciones.Consultas.BalancePorFondo;

/// <summary>
/// Reporte principal de tesorería: cuánto entró y salió por fondo en un rango de fechas.
/// Antes se pensó resolver esto con Dapper; ahora que solo usamos EF Core, se resuelve
/// con un GroupBy de LINQ (ver ITransaccionRepository.ObtenerBalancePorFondoAsync).
/// </summary>
public record BalancePorFondoQuery(DateTime Desde, DateTime Hasta) : IRequest<List<BalancePorFondoRegistro>>;
