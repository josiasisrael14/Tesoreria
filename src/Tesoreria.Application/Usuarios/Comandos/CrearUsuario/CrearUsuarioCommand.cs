using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Comandos.CrearUsuario;

public record CrearUsuarioCommand(string Email, string NombreCompleto, string Contrasena, string Rol) : IRequest<UsuarioDto>;
