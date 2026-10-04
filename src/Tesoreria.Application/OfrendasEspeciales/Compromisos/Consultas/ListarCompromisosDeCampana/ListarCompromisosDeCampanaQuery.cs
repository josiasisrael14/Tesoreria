using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.OfrendasEspeciales.Compromisos;

namespace Tesoreria.Application.OfrendasEspeciales.Compromisos.Consultas.ListarCompromisosDeCampana;

/// <summary>Quién se comprometió a cuánto en esta campaña, y si ya cumplió, está pendiente o en mora.</summary>
public record ListarCompromisosDeCampanaQuery(int OfrendaEspecialId) : IRequest<List<CompromisoOfrendaDto>>;
