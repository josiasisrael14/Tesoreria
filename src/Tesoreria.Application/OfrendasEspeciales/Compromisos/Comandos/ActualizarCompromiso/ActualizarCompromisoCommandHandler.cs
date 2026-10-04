using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Application.OfrendasEspeciales.Compromisos;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.OfrendasEspeciales.Compromisos.Comandos.ActualizarCompromiso;

public class ActualizarCompromisoCommandHandler : IRequestHandler<ActualizarCompromisoCommand, CompromisoOfrendaDto>
{
    private readonly ICompromisoOfrendaRepository _compromisoRepository;
    private readonly ITransaccionRepository _transaccionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarCompromisoCommandHandler(
        ICompromisoOfrendaRepository compromisoRepository,
        ITransaccionRepository transaccionRepository,
        IUnitOfWork unitOfWork)
    {
        _compromisoRepository = compromisoRepository;
        _transaccionRepository = transaccionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CompromisoOfrendaDto> Handle(ActualizarCompromisoCommand request, CancellationToken cancellationToken)
    {
        var compromiso = await _compromisoRepository.ObtenerPorIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException($"No existe el compromiso #{request.Id}.");

        compromiso.SetMonto(request.MontoComprometido);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        var aportes = await _transaccionRepository.ObtenerAportesPorMiembroEnCampanaAsync(compromiso.OfrendaEspecialId, cancellationToken);
        var montoDado = aportes.FirstOrDefault(a => a.MiembroId == compromiso.MiembroId)?.Total ?? 0m;
        var estado = EstadoCompromisoCalculador.Calcular(montoDado, compromiso.MontoComprometido, compromiso.OfrendaEspecial.FechaFin);

        return new CompromisoOfrendaDto(
            compromiso.Id, compromiso.OfrendaEspecialId, compromiso.MiembroId, compromiso.Miembro.NombreCompleto,
            compromiso.MontoComprometido, montoDado, estado);
    }
}
