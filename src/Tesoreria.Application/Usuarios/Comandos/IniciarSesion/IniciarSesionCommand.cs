using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Comandos.IniciarSesion;

public record IniciarSesionCommand(string Email, string Contrasena) : IRequest<SesionDto>;
