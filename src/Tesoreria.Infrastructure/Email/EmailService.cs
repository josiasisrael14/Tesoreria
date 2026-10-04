using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Tesoreria.Application.Abstracciones.Email;

namespace Tesoreria.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task EnviarAsync(
        string destinatarioEmail, string destinatarioNombre, string asunto, string cuerpoHtml,
        CancellationToken cancellationToken = default)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress(_settings.RemitenteNombre, _settings.RemitenteEmail));
        mensaje.To.Add(new MailboxAddress(destinatarioNombre, destinatarioEmail));
        mensaje.Subject = asunto;
        mensaje.Body = new BodyBuilder { HtmlBody = cuerpoHtml }.ToMessageBody();

        using var cliente = new SmtpClient();
        try
        {
            await cliente.ConnectAsync(_settings.SmtpHost, _settings.SmtpPuerto, SecureSocketOptions.StartTls, cancellationToken);
            // Para Gmail, el "usuario" es la cuenta remitente y la "contraseña" es la Contraseña
            // de aplicación de 16 caracteres (no la contraseña normal de la cuenta).
            await cliente.AuthenticateAsync(_settings.RemitenteEmail, _settings.RemitenteContrasena, cancellationToken);
            await cliente.SendAsync(mensaje, cancellationToken);
        }
        finally
        {
            if (cliente.IsConnected)
                await cliente.DisconnectAsync(true, cancellationToken);
        }
    }
}
