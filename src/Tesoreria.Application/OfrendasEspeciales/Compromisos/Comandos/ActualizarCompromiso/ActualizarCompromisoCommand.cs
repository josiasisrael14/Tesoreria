using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.OfrendasEspeciales.Compromisos;

namespace Tesoreria.Application.OfrendasEspeciales.Compromisos.Comandos.ActualizarCompromiso;

/// <summary>Corrige el monto comprometido por un miembro (no se crea uno nuevo).</summary>
public record ActualizarCompromisoCommand(int Id, decimal MontoComprometido) : IRequest<CompromisoOfrendaDto>;
