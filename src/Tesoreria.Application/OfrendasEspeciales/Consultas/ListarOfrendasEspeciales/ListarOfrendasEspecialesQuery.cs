using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.OfrendasEspeciales.Consultas.ListarOfrendasEspeciales;

public record ListarOfrendasEspecialesQuery(bool SoloActivas = true) : IRequest<List<OfrendaEspecialDto>>;
