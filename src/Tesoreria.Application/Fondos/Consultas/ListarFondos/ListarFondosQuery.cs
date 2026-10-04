using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Fondos.Consultas.ListarFondos;

public record ListarFondosQuery(bool SoloActivos = true) : IRequest<List<FondoDto>>;
