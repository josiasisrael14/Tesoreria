using Tesoreria.Application.Usuarios;

namespace Tesoreria.Application.Abstracciones.Identidad;

/// <summary>
/// Abstrae todo lo que necesitamos de ASP.NET Core Identity. Esta capa (Application)
/// no puede depender del paquete de Identity (ver el comentario en
/// Tesoreria.Application.csproj) — por eso esta interfaz solo usa tipos propios
/// (UsuarioDto, ResultadoIdentidad). La implementación real vive en Infrastructure.
/// </summary>
public interface IIdentityService
{
    /// <summary>Null si el correo no existe, la contraseña es incorrecta, o el usuario está desactivado.</summary>
    Task<UsuarioDto?> AutenticarAsync(string email, string contrasena, CancellationToken cancellationToken = default);

    Task<ResultadoIdentidad> CrearUsuarioAsync(
        string email, string nombreCompleto, string contrasena, string rol, CancellationToken cancellationToken = default);

    Task<List<UsuarioDto>> ListarUsuariosAsync(CancellationToken cancellationToken = default);

    Task<ResultadoIdentidad> DesactivarUsuarioAsync(string usuarioId, CancellationToken cancellationToken = default);

    Task<ResultadoIdentidad> ActivarUsuarioAsync(string usuarioId, CancellationToken cancellationToken = default);

    /// <summary>Un Administrador le pone una contraseña nueva a cualquier usuario, sin necesitar la anterior.</summary>
    Task<ResultadoIdentidad> RestablecerContrasenaAsync(
        string usuarioId, string nuevaContrasena, CancellationToken cancellationToken = default);

    /// <summary>El propio usuario cambia su contraseña, confirmando primero la actual.</summary>
    Task<ResultadoIdentidad> CambiarContrasenaAsync(
        string usuarioId, string contrasenaActual, string contrasenaNueva, CancellationToken cancellationToken = default);

    /// <summary>Actualiza la foto de perfil del usuario. Pasar null quita la foto actual.</summary>
    Task<ResultadoIdentidad> ActualizarFotoAsync(
        string usuarioId, string? fotoUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Genera el token de restablecimiento y envía el correo con el enlace. Si el correo no
    /// existe o el usuario está desactivado, no hace nada (no lanza error) — así quien pregunta
    /// no puede averiguar qué correos están registrados en el sistema.
    /// </summary>
    Task SolicitarRecuperacionContrasenaAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completa el restablecimiento usando el token que llegó por correo. Falla con un mensaje
    /// genérico ("El enlace no es válido o ya expiró.") si el token es inválido, ya se usó, o ya
    /// expiró — nunca expone el detalle interno de Identity.
    /// </summary>
    Task<ResultadoIdentidad> RestablecerContrasenaConTokenAsync(
        string email, string token, string nuevaContrasena, CancellationToken cancellationToken = default);
}

/// <summary>Resultado de una operación de Identity (crear/activar/desactivar/contraseña), sin exponer tipos de Identity.</summary>
public record ResultadoIdentidad(bool Exitoso, IReadOnlyCollection<string> Errores, UsuarioDto? Usuario = null)
{
    public static ResultadoIdentidad Ok(UsuarioDto? usuario = null) => new(true, Array.Empty<string>(), usuario);

    public static ResultadoIdentidad Falla(IEnumerable<string> errores) => new(false, errores.ToList());
}
