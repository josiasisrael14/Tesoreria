using Microsoft.AspNetCore.Mvc;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.OfrendasEspeciales.Comandos.CrearOfrendaEspecial;
using Tesoreria.Application.OfrendasEspeciales.Comandos.DesactivarOfrendaEspecial;
using Tesoreria.Application.OfrendasEspeciales.Consultas.ListarOfrendasEspeciales;
using Tesoreria.Application.OfrendasEspeciales.Compromisos.Comandos.ActualizarCompromiso;
using Tesoreria.Application.OfrendasEspeciales.Compromisos.Comandos.EliminarCompromiso;
using Tesoreria.Application.OfrendasEspeciales.Compromisos.Comandos.RegistrarCompromiso;
using Tesoreria.Application.OfrendasEspeciales.Compromisos.Consultas.ListarCompromisosDeCampana;

namespace Tesoreria.Api.Controllers;

/// <summary>Catálogo de campañas de ofrenda especial: ofrenda misionera, pro templo, aniversario, etc.</summary>
[ApiController]
[Route("api/[controller]")]
public class OfrendasEspecialesController : ControllerBase
{
    private readonly IMediator _mediator;

    public OfrendasEspecialesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] bool soloActivas = true, CancellationToken cancellationToken = default)
    {
        var ofrendas = await _mediator.Send(new ListarOfrendasEspecialesQuery(soloActivas), cancellationToken);
        return Ok(ofrendas);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearOfrendaEspecialCommand comando, CancellationToken cancellationToken)
    {
        var ofrenda = await _mediator.Send(comando, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { id = ofrenda.Id }, ofrenda);
    }

    [HttpPost("{id:int}/desactivar")]
    public async Task<IActionResult> Desactivar(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DesactivarOfrendaEspecialCommand(id), cancellationToken);
        return Ok();
    }

    /// <summary>Quién se comprometió a cuánto en esta campaña, y si ya cumplió, está pendiente o en mora.</summary>
    [HttpGet("{ofrendaEspecialId:int}/compromisos")]
    public async Task<IActionResult> ListarCompromisos(int ofrendaEspecialId, CancellationToken cancellationToken)
    {
        var compromisos = await _mediator.Send(new ListarCompromisosDeCampanaQuery(ofrendaEspecialId), cancellationToken);
        return Ok(compromisos);
    }

    [HttpPost("{ofrendaEspecialId:int}/compromisos")]
    public async Task<IActionResult> RegistrarCompromiso(
        int ofrendaEspecialId, [FromBody] RegistrarCompromisoPeticion peticion, CancellationToken cancellationToken)
    {
        var comando = new RegistrarCompromisoCommand(ofrendaEspecialId, peticion.MiembroId, peticion.MontoComprometido);
        var compromiso = await _mediator.Send(comando, cancellationToken);
        return Ok(compromiso);
    }

    [HttpPut("compromisos/{id:int}")]
    public async Task<IActionResult> ActualizarCompromiso(
        int id, [FromBody] ActualizarCompromisoPeticion peticion, CancellationToken cancellationToken)
    {
        var compromiso = await _mediator.Send(new ActualizarCompromisoCommand(id, peticion.MontoComprometido), cancellationToken);
        return Ok(compromiso);
    }

    [HttpDelete("compromisos/{id:int}")]
    public async Task<IActionResult> EliminarCompromiso(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new EliminarCompromisoCommand(id), cancellationToken);
        return Ok();
    }

    public record RegistrarCompromisoPeticion(int MiembroId, decimal MontoComprometido);
    public record ActualizarCompromisoPeticion(decimal MontoComprometido);
}
