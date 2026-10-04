using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Miembros.Consultas.HistorialAportesMiembro;

/// <summary>
/// Cuánto ha dado un miembro en total: por fondo (diezmos, ofrenda general...) y,
/// dentro de ofrendas especiales, por campaña (misionera, pro templo, aniversario...).
/// </summary>
public record HistorialAportesMiembroQuery(int MiembroId) : IRequest<HistorialAportesMiembroDto>;
