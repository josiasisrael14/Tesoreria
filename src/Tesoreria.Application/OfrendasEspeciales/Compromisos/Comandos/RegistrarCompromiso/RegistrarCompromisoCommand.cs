using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.OfrendasEspeciales.Compromisos;

namespace Tesoreria.Application.OfrendasEspeciales.Compromisos.Comandos.RegistrarCompromiso;

/// <summary>Un miembro se compromete a dar un monto para una campaña de ofrenda especial.</summary>
public record RegistrarCompromisoCommand(
    int OfrendaEspecialId,
    int MiembroId,
    decimal MontoComprometido) : IRequest<CompromisoOfrendaDto>;
