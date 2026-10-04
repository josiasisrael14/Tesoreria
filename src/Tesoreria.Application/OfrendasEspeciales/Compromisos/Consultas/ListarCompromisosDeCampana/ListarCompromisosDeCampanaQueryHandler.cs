using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Application.OfrendasEspeciales.Compromisos;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.OfrendasEspeciales.Compromisos.Consultas.ListarCompromisosDeCampana;

public class ListarCompromisosDeCampanaQueryHandler
    : IRequestHandler<ListarCompromisosDeCampanaQuery, List<CompromisoOfrendaDto>>
{
    private readonly ICompromisoOfrendaRepository _compromisoRepository;
    private readonly IOfrendaEspecialRepository _ofrendaEspecialRepository;
    private readonly ITransaccionRepository _transaccionRepository;

    public ListarCompromisosDeCampanaQueryHandler(
        ICompromisoOfrendaRepository compromisoRepository,
        IOfrendaEspecialRepository ofrendaEspecialRepository,
        ITransaccionRepository transaccionRepository)
    {
        _compromisoRepository = compromisoRepository;
        _ofrendaEspecialRepository = ofrendaEspecialRepository;
        _transaccionRepository = transaccionRepository;
    }

    public async Task<List<CompromisoOfrendaDto>> Handle(ListarCompromisosDeCampanaQuery request, CancellationToken cancellationToken)
    {
        var ofrenda = await _ofrendaEspecialRepository.ObtenerPorIdAsync(request.OfrendaEspecialId, cancellationToken)
            ?? throw new DomainException($"No existe la campaña #{request.OfrendaEspecialId}.");

        var compromisos = await _compromisoRepository.ListarPorOfrendaEspecialAsync(request.OfrendaEspecialId, cancellationToken);
        var aportes = await _transaccionRepository.ObtenerAportesPorMiembroEnCampanaAsync(request.OfrendaEspecialId, cancellationToken);
        var montoDadoPorMiembro = aportes.ToDictionary(a => a.MiembroId, a => a.Total);

        return compromisos
            .Select(c =>
            {
                var montoDado = montoDadoPorMiembro.GetValueOrDefault(c.MiembroId, 0m);
                var estado = EstadoCompromisoCalculador.Calcular(montoDado, c.MontoComprometido, ofrenda.FechaFin);
                return new CompromisoOfrendaDto(
                    c.Id, c.OfrendaEspecialId, c.MiembroId, c.Miembro.NombreCompleto, c.MontoComprometido, montoDado, estado);
            })
            .OrderBy(d => d.NombreMiembro)
            .ToList();
    }
}
