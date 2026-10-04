namespace Tesoreria.Application.OfrendasEspeciales;

public record OfrendaEspecialDto(
    int Id,
    string Nombre,
    int Anio,
    string? Descripcion,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    decimal? MetaMonto,
    bool Activo);
