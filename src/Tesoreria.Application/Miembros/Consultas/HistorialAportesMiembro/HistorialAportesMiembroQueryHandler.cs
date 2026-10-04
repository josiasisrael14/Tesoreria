using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Miembros.Consultas.HistorialAportesMiembro;

public class HistorialAportesMiembroQueryHandler
    : IRequestHandler<HistorialAportesMiembroQuery, HistorialAportesMiembroDto>
{
    private readonly IMiembroRepository _miembroRepository;
    private readonly ITransaccionRepository _transaccionRepository;

    public HistorialAportesMiembroQueryHandler(
        IMiembroRepository miembroRepository, ITransaccionRepository transaccionRepository)
    {
        _miembroRepository = miembroRepository;
        _transaccionRepository = transaccionRepository;
    }

    public async Task<HistorialAportesMiembroDto> Handle(HistorialAportesMiembroQuery request, CancellationToken cancellationToken)
    {
        var miembro = await _miembroRepository.ObtenerPorIdAsync(request.MiembroId, cancellationToken)
            ?? throw new DomainException($"No existe el miembro #{request.MiembroId}.");

        var aportesPorFondo = await _transaccionRepository.ObtenerAportesPorFondoDeMiembroAsync(request.MiembroId, cancellationToken);
        var aportesPorOfrendaEspecial = await _transaccionRepository.ObtenerAportesPorOfrendaEspecialDeMiembroAsync(request.MiembroId, cancellationToken);

        return new HistorialAportesMiembroDto(
            miembro.Id,
            miembro.NombreCompleto,
            aportesPorFondo
                .Select(a => new AporteFondoDto(a.FondoId, a.NombreFondo, a.Total))
                .OrderBy(a => a.NombreFondo)
                .ToList(),
            aportesPorOfrendaEspecial
                .Select(a => new AporteOfrendaEspecialDto(a.OfrendaEspecialId, a.NombreOfrendaEspecial, a.Total))
                .OrderBy(a => a.NombreOfrendaEspecial)
                .ToList());
    }
}
