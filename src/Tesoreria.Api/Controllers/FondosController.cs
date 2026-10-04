using Microsoft.AspNetCore.Mvc;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Fondos.Comandos.CrearFondo;
using Tesoreria.Application.Fondos.Comandos.DesactivarFondo;
using Tesoreria.Application.Fondos.Comandos.EditarFondo;
using Tesoreria.Application.Fondos.Consultas.ListarFondos;
using Tesoreria.Domain.Enums;

namespace Tesoreria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FondosController : ControllerBase
{
    private readonly IMediator _mediator;

    public FondosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Lista los fondos (Diezmo, Ofrenda General, Ofrenda Especial, Misiones, etc.).</summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] bool soloActivos = true, CancellationToken cancellationToken = default)
    {
        var fondos = await _mediator.Send(new ListarFondosQuery(soloActivos), cancellationToken);
        return Ok(fondos);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearFondoCommand comando, CancellationToken cancellationToken)
    {
        var fondo = await _mediator.Send(comando, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { id = fondo.Id }, fondo);
    }

    /// <summary>Corrige en el sitio los datos de un fondo (por ejemplo, un nombre mal escrito al crearlo).</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] EditarFondoPeticion peticion, CancellationToken cancellationToken)
    {
        var comando = new EditarFondoCommand(id, peticion.Nombre, peticion.Tipo, peticion.Descripcion, peticion.EsFondoDeOfrendasEspeciales);
        var fondo = await _mediator.Send(comando, cancellationToken);
        return Ok(fondo);
    }

    [HttpPost("{id:int}/desactivar")]
    public async Task<IActionResult> Desactivar(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DesactivarFondoCommand(id), cancellationToken);
        return NoContent();
    }

    public record EditarFondoPeticion(string Nombre, TipoFondo Tipo, string? Descripcion, bool EsFondoDeOfrendasEspeciales);
}
