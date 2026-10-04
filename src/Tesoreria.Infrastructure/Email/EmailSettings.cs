namespace Tesoreria.Infrastructure.Email;

/// <summary>
/// Mapea la sección "Email" de appsettings.json: la cuenta remitente por SMTP (ej. un Gmail
/// con una Contraseña de aplicación) y la URL del frontend, para armar el enlace que se manda
/// en el correo de "restablecer contraseña".
/// </summary>
public class EmailSettings
{
    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPuerto { get; set; } = 587;
    public string RemitenteEmail { get; set; } = default!;
    public string RemitenteContrasena { get; set; } = default!;
    public string RemitenteNombre { get; set; } = "Tesorería";

    /// <summary>Base de la URL del frontend (sin "/" al final), ej. "https://tesoreria.tuiglesia.com".</summary>
    public string FrontendBaseUrl { get; set; } = default!;
}
