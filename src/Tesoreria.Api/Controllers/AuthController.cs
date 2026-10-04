using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Tesoreria.Api.Archivos;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Usuarios.Comandos.ActualizarFoto;
using Tesoreria.Application.Usuarios.Comandos.CambiarContrasena;
using Tesoreria.Application.Usuarios.Comandos.IniciarSesion;
using Tesoreria.Application.Usuarios.Comandos.RestablecerContrasenaConToken;
using Tesoreria.Application.Usuarios.Comandos.SolicitarRecuperacionContrasena;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private static readonly string[] ExtensionesSoportadas = { "jpg", "png", "webp" };

    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _entornoWeb;

    public AuthController(IMediator mediator, IWebHostEnvironment entornoWeb)
    {
        _mediator = mediator;
        _entornoWeb = entornoWeb;
    }

    /// <summary>Único endpoint público de todo el sistema: todo lo demás exige un token válido.</summary>
    [AllowAnonymous]
    [HttpPost("iniciar-sesion")]
    public async Task<IActionResult> IniciarSesion([FromBody] IniciarSesionCommand comando, CancellationToken cancellationToken)
    {
        var sesion = await _mediator.Send(comando, cancellationToken);
        return Ok(sesion);
    }

    /// <summary>
    /// Pide el enlace de recuperación por correo. Siempre responde el mismo mensaje genérico,
    /// exista o no ese correo en el sistema — así nadie puede usar este endpoint para averiguar
    /// qué correos están registrados (ver el comentario en SolicitarRecuperacionContrasenaCommandHandler).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("olvide-mi-contrasena")]
    public async Task<IActionResult> OlvideMiContrasena(
        [FromBody] SolicitarRecuperacionContrasenaCommand comando, CancellationToken cancellationToken)
    {
        await _mediator.Send(comando, cancellationToken);
        return Ok(new { mensaje = "Si ese correo está registrado, te enviamos un enlace para restablecer tu contraseña." });
    }

    /// <summary>Completa el restablecimiento con el token recibido por correo. No requiere haber iniciado sesión.</summary>
    [AllowAnonymous]
    [HttpPost("restablecer-contrasena-con-token")]
    public async Task<IActionResult> RestablecerContrasenaConToken(
        [FromBody] RestablecerContrasenaConTokenCommand comando, CancellationToken cancellationToken)
    {
        await _mediator.Send(comando, cancellationToken);
        return NoContent();
    }

    /// <summary>El propio usuario (ya logueado) cambia su contraseña desde "Mi perfil". Exige la contraseña actual.</summary>
    [HttpPost("cambiar-contrasena")]
    public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaPeticion peticion, CancellationToken cancellationToken)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        await _mediator.Send(
            new CambiarContrasenaCommand(usuarioId, peticion.ContrasenaActual, peticion.ContrasenaNueva), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Sube (o reemplaza) la foto de perfil del usuario logueado. El archivo se valida por su
    /// contenido real (ver ValidadorImagen) — no por su nombre ni por el Content-Type que manda
    /// el navegador, porque ambos los puede poner cualquiera con cualquier valor. El nombre final
    /// en disco lo decide el servidor (id del usuario + extensión detectada), nunca el nombre que
    /// vino en la petición, así que no hay forma de que esto permita escribir fuera de esa carpeta.
    /// </summary>
    [HttpPost("mi-foto")]
    public async Task<IActionResult> SubirFoto(IFormFile? archivo, CancellationToken cancellationToken)
    {
        if (archivo is null)
            throw new DomainException("No se recibió ningún archivo.");

        var (esValida, extension, error) = await ValidadorImagen.ValidarAsync(archivo, cancellationToken);
        if (!esValida)
            throw new DomainException(error ?? "El archivo no es una imagen válida.");

        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var carpetaUploads = ObtenerCarpetaUploads();
        Directory.CreateDirectory(carpetaUploads);

        // Borra versiones anteriores con otra extensión (ej. tenía .png y ahora sube un .jpg),
        // para no dejar fotos viejas huérfanas en el disco.
        BorrarFotosExistentes(carpetaUploads, usuarioId);

        var nombreArchivo = $"{usuarioId}.{extension}";
        var rutaDestino = Path.Combine(carpetaUploads, nombreArchivo);

        await using (var destino = System.IO.File.Create(rutaDestino))
        {
            await using var origen = archivo.OpenReadStream();
            await origen.CopyToAsync(destino, cancellationToken);
        }

        // El "?v=" es cache-busting: sin esto, el navegador podría seguir mostrando la foto
        // anterior desde su caché aunque el archivo en el servidor ya haya cambiado.
        var fotoUrl = $"/uploads/usuarios/{nombreArchivo}?v={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        var usuario = await _mediator.Send(new ActualizarFotoCommand(usuarioId, fotoUrl), cancellationToken);
        return Ok(usuario);
    }

    /// <summary>Quita la foto de perfil del usuario logueado; vuelve a mostrarse el ícono con su inicial.</summary>
    [HttpDelete("mi-foto")]
    public async Task<IActionResult> QuitarFoto(CancellationToken cancellationToken)
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        BorrarFotosExistentes(ObtenerCarpetaUploads(), usuarioId);

        var usuario = await _mediator.Send(new ActualizarFotoCommand(usuarioId, null), cancellationToken);
        return Ok(usuario);
    }

    private string ObtenerCarpetaUploads()
    {
        var carpetaRaiz = _entornoWeb.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
        return Path.Combine(carpetaRaiz, "uploads", "usuarios");
    }

    private static void BorrarFotosExistentes(string carpetaUploads, string usuarioId)
    {
        foreach (var extension in ExtensionesSoportadas)
        {
            var ruta = Path.Combine(carpetaUploads, $"{usuarioId}.{extension}");
            if (System.IO.File.Exists(ruta))
                System.IO.File.Delete(ruta);
        }
    }

    public record CambiarContrasenaPeticion(string ContrasenaActual, string ContrasenaNueva);
}
