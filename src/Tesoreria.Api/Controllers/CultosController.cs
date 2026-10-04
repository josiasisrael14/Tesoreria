using Microsoft.AspNetCore.Mvc;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Cultos.Comandos.CrearCulto;
using Tesoreria.Application.Cultos.Consultas.ListarCultos;

namespace Tesoreria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CultosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CultosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken cancellationToken)
    {
        var cultos = await _mediator.Send(new ListarCultosQuery(desde, hasta), cancellationToken);
        return Ok(cultos);
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearCultoCommand comando, CancellationToken cancellationToken)
    {
        var culto = await _mediator.Send(comando, cancellationToken);
        return CreatedAtAction(nameof(Listar), new { id = culto.Id }, culto);
    }
}
