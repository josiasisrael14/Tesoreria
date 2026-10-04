using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Entidades;

namespace Tesoreria.Application.Cultos.Comandos.CrearCulto;

public record CrearCultoCommand(DateTime Fecha, TipoCulto Tipo, string? Descripcion) : IRequest<CultoDto>;
