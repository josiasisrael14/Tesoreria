using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Cultos.Consultas.ListarCultos;

public record ListarCultosQuery(DateTime? Desde, DateTime? Hasta) : IRequest<List<CultoDto>>;
