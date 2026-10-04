using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.OfrendasEspeciales.Comandos.DesactivarOfrendaEspecial;

public record DesactivarOfrendaEspecialCommand(int OfrendaEspecialId) : IRequest<bool>;
