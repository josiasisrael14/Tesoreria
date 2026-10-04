using Tesoreria.Domain.Entidades;

namespace Tesoreria.Application.Cultos;

public record CultoDto(int Id, DateTime Fecha, TipoCulto Tipo, string? Descripcion);
