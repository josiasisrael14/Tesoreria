using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Tesoreria.Application.Abstracciones.Email;
using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Usuarios;
using Tesoreria.Infrastructure.Email;

namespace Tesoreria.Infrastructure.Identidad;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;

    public IdentityService(
        UserManager<ApplicationUser> userManager, IEmailService emailService, IOptions<EmailSettings> emailSettings)
    {
        _userManager = userManager;
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
    }

    public async Task<UsuarioDto?> AutenticarAsync(string email, string contrasena, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByEmailAsync(email);
        if (usuario is null || !usuario.Activo)
            return null;

        var contrasenaValida = await _userManager.CheckPasswordAsync(usuario, contrasena);
        if (!contrasenaValida)
            return null;

        return await MapearAsync(usuario);
    }

    public async Task<ResultadoIdentidad> CrearUsuarioAsync(
        string email, string nombreCompleto, string contrasena, string rol, CancellationToken cancellationToken = default)
    {
        var usuario = new ApplicationUser
        {
            UserName = email,
            Email = email,
            NombreCompleto = nombreCompleto,
            Activo = true
        };

        var resultado = await _userManager.CreateAsync(usuario, contrasena);
        if (!resultado.Succeeded)
            return ResultadoIdentidad.Falla(resultado.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(usuario, rol);

        return ResultadoIdentidad.Ok(await MapearAsync(usuario));
    }

    public async Task<List<UsuarioDto>> ListarUsuariosAsync(CancellationToken cancellationToken = default)
    {
        var usuarios = _userManager.Users.OrderBy(u => u.NombreCompleto).ToList();

        var resultado = new List<UsuarioDto>();
        foreach (var usuario in usuarios)
            resultado.Add(await MapearAsync(usuario));

        return resultado;
    }

    public async Task<ResultadoIdentidad> DesactivarUsuarioAsync(string usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByIdAsync(usuarioId);
        if (usuario is null)
            return ResultadoIdentidad.Falla(new[] { "No existe ese usuario." });

        usuario.Activo = false;
        var resultado = await _userManager.UpdateAsync(usuario);

        return resultado.Succeeded
            ? ResultadoIdentidad.Ok(await MapearAsync(usuario))
            : ResultadoIdentidad.Falla(resultado.Errors.Select(e => e.Description));
    }

    public async Task<ResultadoIdentidad> ActivarUsuarioAsync(string usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByIdAsync(usuarioId);
        if (usuario is null)
            return ResultadoIdentidad.Falla(new[] { "No existe ese usuario." });

        usuario.Activo = true;
        var resultado = await _userManager.UpdateAsync(usuario);

        return resultado.Succeeded
            ? ResultadoIdentidad.Ok(await MapearAsync(usuario))
            : ResultadoIdentidad.Falla(resultado.Errors.Select(e => e.Description));
    }

    public async Task<ResultadoIdentidad> RestablecerContrasenaAsync(
        string usuarioId, string nuevaContrasena, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByIdAsync(usuarioId);
        if (usuario is null)
            return ResultadoIdentidad.Falla(new[] { "No existe ese usuario." });

        // GeneratePasswordResetToken + ResetPassword es la forma estándar de Identity para que
        // un Administrador le ponga una contraseña nueva a otro usuario sin saber la anterior
        // (a diferencia de ChangePasswordAsync, que sí la exige — esa es para el propio usuario).
        var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
        var resultado = await _userManager.ResetPasswordAsync(usuario, token, nuevaContrasena);

        return resultado.Succeeded
            ? ResultadoIdentidad.Ok(await MapearAsync(usuario))
            : ResultadoIdentidad.Falla(resultado.Errors.Select(e => e.Description));
    }

    public async Task<ResultadoIdentidad> CambiarContrasenaAsync(
        string usuarioId, string contrasenaActual, string contrasenaNueva, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByIdAsync(usuarioId);
        if (usuario is null)
            return ResultadoIdentidad.Falla(new[] { "No existe ese usuario." });

        var resultado = await _userManager.ChangePasswordAsync(usuario, contrasenaActual, contrasenaNueva);

        return resultado.Succeeded
            ? ResultadoIdentidad.Ok(await MapearAsync(usuario))
            : ResultadoIdentidad.Falla(resultado.Errors.Select(e => e.Description));
    }

    public async Task<ResultadoIdentidad> ActualizarFotoAsync(
        string usuarioId, string? fotoUrl, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByIdAsync(usuarioId);
        if (usuario is null)
            return ResultadoIdentidad.Falla(new[] { "No existe ese usuario." });

        usuario.FotoUrl = fotoUrl;
        var resultado = await _userManager.UpdateAsync(usuario);

        return resultado.Succeeded
            ? ResultadoIdentidad.Ok(await MapearAsync(usuario))
            : ResultadoIdentidad.Falla(resultado.Errors.Select(e => e.Description));
    }

    public async Task SolicitarRecuperacionContrasenaAsync(string email, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByEmailAsync(email);
        if (usuario is null || !usuario.Activo)
            return;

        var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);

        var emailCodificado = WebUtility.UrlEncode(usuario.Email);
        var tokenCodificado = WebUtility.UrlEncode(token);
        var enlace = $"{_emailSettings.FrontendBaseUrl}/restablecer-contrasena?email={emailCodificado}&token={tokenCodificado}";

        var cuerpoHtml = $"""
            <p>Hola {usuario.NombreCompleto},</p>
            <p>Recibimos una solicitud para restablecer tu contraseña en Tesorería.</p>
            <p><a href="{enlace}">Haz clic aquí para crear una nueva contraseña</a></p>
            <p>Este enlace es válido por 2 horas. Si tú no solicitaste esto, puedes ignorar este correo.</p>
            """;

        await _emailService.EnviarAsync(
            usuario.Email!, usuario.NombreCompleto, "Recuperación de contraseña - Tesorería", cuerpoHtml, cancellationToken);
    }

    public async Task<ResultadoIdentidad> RestablecerContrasenaConTokenAsync(
        string email, string token, string nuevaContrasena, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByEmailAsync(email);
        if (usuario is null)
            return ResultadoIdentidad.Falla(new[] { "El enlace no es válido o ya expiró." });

        var resultado = await _userManager.ResetPasswordAsync(usuario, token, nuevaContrasena);

        return resultado.Succeeded
            ? ResultadoIdentidad.Ok(await MapearAsync(usuario))
            : ResultadoIdentidad.Falla(new[] { "El enlace no es válido o ya expiró." });
    }

    private async Task<UsuarioDto> MapearAsync(ApplicationUser usuario)
    {
        var roles = await _userManager.GetRolesAsync(usuario);
        var rol = roles.FirstOrDefault() ?? string.Empty;

        return new UsuarioDto(usuario.Id, usuario.Email!, usuario.NombreCompleto, rol, usuario.Activo, usuario.FotoUrl);
    }
}
