using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Comandos.ActivarUsuario;

public record ActivarUsuarioCommand(string UsuarioId) : IRequest<bool>;
