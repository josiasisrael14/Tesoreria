using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Fondos.Comandos.DesactivarFondo;

public record DesactivarFondoCommand(int FondoId) : IRequest<bool>;
