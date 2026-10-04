using Microsoft.AspNetCore.Mvc;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Miembros.Comandos.ActivarMiembro;
using Tesoreria.Application.Miembros.Comandos.ActualizarMiembro;
using Tesoreria.Application.Miembros.Comandos.CrearMiembro;
using Tesoreria.Application.Miembros.Comandos.DesactivarMiembro;
using Tesoreria.Application.Miembros.Comandos.ImportarMiembros;
using Tesoreria.Application.Miembros.Consultas.HistorialAportesMiembro;
using Tesoreria.Application.Miembros.Consultas.ListarMiembros;

namespace Tesoreria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MiembrosController : ControllerBase
{
    private readonly IMediator _mediator;

    public MiembrosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] bool soloActivos = true,
        [FromQuery] string? busqueda = null,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _mediator.Send(
            new ListarMiembrosQuery(soloActivos, busqueda, pagina, tamanoPagina), cancellationToken);
        return Ok(resultado);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearMiembroCommand comando, CancellationToken cancellationToken)
    {
        var miembro = await _mediator.Send(comando, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { id = miembro.Id }, miembro);
    }

    /// <summary>Crea muchos miembros de una sola vez a partir de un Excel que el usuario subió
    /// (el parseo del archivo ya lo hizo el frontend). Las filas inválidas no frenan a las demás:
    /// se crean las que sí sirven y se informa el detalle de las que no.</summary>
    [HttpPost("importar")]
    public async Task<IActionResult> Importar([FromBody] ImportarMiembrosCommand comando, CancellationToken cancellationToken)
    {
        var resultado = await _mediator.Send(comando, cancellationToken);
        return Ok(resultado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarMiembroCommand comando, CancellationToken cancellationToken)
    {
        if (id != comando.Id)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el id del miembro a actualizar." });

        var miembro = await _mediator.Send(comando, cancellationToken);
        return Ok(miembro);
    }

    /// <summary>"Elimina" un miembro de forma lógica: lo desactiva (Activo pasa a false), no borra el registro.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DesactivarMiembroCommand(id), cancellationToken);
        return Ok();
    }

    /// <summary>Reactiva a un miembro que había sido desactivado.</summary>
    [HttpPost("{id:int}/activar")]
    public async Task<IActionResult> Activar(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ActivarMiembroCommand(id), cancellationToken);
        return Ok();
    }

    /// <summary>Cuánto ha dado este miembro: por fondo (diezmos, ofrenda general...) y por campaña de ofrenda especial.</summary>
    [HttpGet("{id:int}/aportes")]
    public async Task<IActionResult> Aportes(int id, CancellationToken cancellationToken)
    {
        var historial = await _mediator.Send(new HistorialAportesMiembroQuery(id), cancellationToken);
        return Ok(historial);
    }
}
