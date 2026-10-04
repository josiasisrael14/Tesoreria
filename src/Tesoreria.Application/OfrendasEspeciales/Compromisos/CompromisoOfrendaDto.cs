namespace Tesoreria.Application.OfrendasEspeciales.Compromisos;

/// <summary>
/// Estado = "Cumplido" | "Pendiente" | "EnMora" — se calcula al vuelo comparando
/// MontoDado contra MontoComprometido y la fecha límite de la campaña (ver
/// EstadoCompromisoCalculador). No se guarda en la base de datos.
/// </summary>
public record CompromisoOfrendaDto(
    int Id,
    int OfrendaEspecialId,
    int MiembroId,
    string NombreMiembro,
    decimal MontoComprometido,
    decimal MontoDado,
    string Estado);
