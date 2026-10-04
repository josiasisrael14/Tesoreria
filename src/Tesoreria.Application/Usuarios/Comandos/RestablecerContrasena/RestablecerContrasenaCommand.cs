using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Comandos.RestablecerContrasena;

/// <summary>Usado por un Administrador para ponerle una contraseña nueva a cualquier usuario (ej. se le olvidó la suya).</summary>
public record RestablecerContrasenaCommand(string UsuarioId, string NuevaContrasena) : IRequest<bool>;
