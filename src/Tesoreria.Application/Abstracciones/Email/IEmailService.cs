namespace Tesoreria.Application.Abstracciones.Email;

/// <summary>
/// Abstrae el envío de correos. Igual que con IIdentityService, esta capa (Application) no
/// depende de ninguna librería de correo concreta (ver el comentario en
/// Tesoreria.Application.csproj) — la implementación real (SMTP) vive en Infrastructure.
/// </summary>
public interface IEmailService
{
    Task EnviarAsync(
        string destinatarioEmail, string destinatarioNombre, string asunto, string cuerpoHtml,
        CancellationToken cancellationToken = default);
}
