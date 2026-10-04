using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Comandos.SolicitarRecuperacionContrasena;

public record SolicitarRecuperacionContrasenaCommand(string Email) : IRequest<bool>;
