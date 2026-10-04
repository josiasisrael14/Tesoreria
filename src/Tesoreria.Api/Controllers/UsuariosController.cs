using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Usuarios;
using Tesoreria.Application.Usuarios.Comandos.ActivarUsuario;
using Tesoreria.Application.Usuarios.Comandos.CrearUsuario;
using Tesoreria.Application.Usuarios.Comandos.DesactivarUsuario;
using Tesoreria.Application.Usuarios.Comandos.RestablecerContrasena;
using Tesoreria.Application.Usuarios.Consultas.ListarUsuarios;

namespace Tesoreria.Api.Controllers;

/// <summary>Gestión de usuarios: solo un Administrador puede crear, listar, activar, desactivar o restablecer contraseñas.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Administrador)]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var usuarios = await _mediator.Send(new ListarUsuariosQuery(), cancellationToken);
        return Ok(usuarios);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUsuarioCommand comando, CancellationToken cancellationToken)
    {
        var usuario = await _mediator.Send(comando, cancellationToken);
        return CreatedAtAction(nameof(Listar), usuario);
    }

    [HttpPost("{id}/desactivar")]
    public async Task<IActionResult> Desactivar(string id, CancellationToken cancellationToken)
    {
        var usuarioActualId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        await _mediator.Send(new DesactivarUsuarioCommand(id, usuarioActualId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/activar")]
    public async Task<IActionResult> Activar(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ActivarUsuarioCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Le pone una contraseña nueva a un usuario que olvidó la suya. No requiere la anterior.</summary>
    [HttpPost("{id}/restablecer-contrasena")]
    public async Task<IActionResult> RestablecerContrasena(
        string id, [FromBody] RestablecerContrasenaPeticion peticion, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RestablecerContrasenaCommand(id, peticion.NuevaContrasena), cancellationToken);
        return NoContent();
    }

    public record RestablecerContrasenaPeticion(string NuevaContrasena);
}
