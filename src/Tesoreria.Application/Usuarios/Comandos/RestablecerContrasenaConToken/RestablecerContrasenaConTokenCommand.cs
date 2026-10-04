using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Comandos.RestablecerContrasenaConToken;

/// <summary>
/// Completa el restablecimiento usando el token que llegó por correo — a diferencia de
/// CambiarContrasenaCommand (pide la contraseña actual) y RestablecerContrasenaCommand (lo hace
/// un Administrador), este no requiere haber iniciado sesión ni conocer la contraseña anterior.
/// </summary>
public record RestablecerContrasenaConTokenCommand(string Email, string Token, string NuevaContrasena) : IRequest<bool>;
