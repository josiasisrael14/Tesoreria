using Microsoft.AspNetCore.Mvc;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Transacciones.Comandos.EditarTransaccion;
using Tesoreria.Application.Transacciones.Comandos.RegistrarEgreso;
using Tesoreria.Application.Transacciones.Comandos.RegistrarIngreso;
using Tesoreria.Application.Transacciones.Consultas.ListarTransacciones;
using Tesoreria.Domain.Enums;

namespace Tesoreria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransaccionesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransaccionesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int? fondoId,
        [FromQuery] int? cultoId,
        [FromQuery] int? miembroId,
        [FromQuery] int? ofrendaEspecialId,
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        [FromQuery] MedioPago? medioPago,
        CancellationToken cancellationToken)
    {
        var query = new ListarTransaccionesQuery(fondoId, cultoId, miembroId, ofrendaEspecialId, desde, hasta, medioPago);
        var transacciones = await _mediator.Send(query, cancellationToken);
        return Ok(transacciones);
    }

    /// <summary>La pantalla de "registro rápido": lo que se recoge en un culto.</summary>
    [HttpPost("ingresos")]
    public async Task<IActionResult> RegistrarIngreso([FromBody] RegistrarIngresoCommand comando, CancellationToken cancellationToken)
    {
        var transaccion = await _mediator.Send(comando, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { id = transaccion.Id }, transaccion);
    }

    [HttpPost("egresos")]
    public async Task<IActionResult> RegistrarEgreso([FromBody] RegistrarEgresoCommand comando, CancellationToken cancellationToken)
    {
        var transaccion = await _mediator.Send(comando, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { id = transaccion.Id }, transaccion);
    }

    /// <summary>Corrige en el sitio los datos de una transacción ya guardada (error de tipeo en el monto, el fondo, el miembro, etc.).</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] EditarTransaccionPeticion peticion, CancellationToken cancellationToken)
    {
        var comando = new EditarTransaccionCommand(
            id, peticion.Fecha, peticion.FondoId, peticion.Monto, peticion.MedioPago,
            peticion.MiembroId, peticion.CultoId, peticion.OfrendaEspecialId,
            peticion.Concepto, peticion.NumeroComprobante);
        var transaccion = await _mediator.Send(comando, cancellationToken);
        return Ok(transaccion);
    }

    public record EditarTransaccionPeticion(
        DateTime Fecha,
        int FondoId,
        decimal Monto,
        MedioPago MedioPago,
        int? MiembroId,
        int? CultoId,
        int? OfrendaEspecialId,
        string? Concepto,
        string? NumeroComprobante);
}
