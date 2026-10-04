using Tesoreria.Domain.Enums;

namespace Tesoreria.Application.Fondos;

public record FondoDto(int Id, string Nombre, string? Descripcion, TipoFondo Tipo, bool Activo, bool EsFondoDeOfrendasEspeciales);
