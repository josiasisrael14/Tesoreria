using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Comandos.CambiarContrasena;

/// <summary>Usado por el propio usuario para cambiar su contraseña, desde "Mi perfil". Exige la contraseña actual.</summary>
public record CambiarContrasenaCommand(string UsuarioId, string ContrasenaActual, string ContrasenaNueva) : IRequest<bool>;
