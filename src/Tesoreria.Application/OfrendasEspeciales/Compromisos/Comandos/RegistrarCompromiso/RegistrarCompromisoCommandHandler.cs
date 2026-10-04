using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Application.OfrendasEspeciales.Compromisos;
using Tesoreria.Domain.Entidades;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.OfrendasEspeciales.Compromisos.Comandos.RegistrarCompromiso;

public class RegistrarCompromisoCommandHandler : IRequestHandler<RegistrarCompromisoCommand, CompromisoOfrendaDto>
{
    private readonly ICompromisoOfrendaRepository _compromisoRepository;
    private readonly IOfrendaEspecialRepository _ofrendaEspecialRepository;
    private readonly IMiembroRepository _miembroRepository;
    private readonly ITransaccionRepository _transaccionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarCompromisoCommandHandler(
        ICompromisoOfrendaRepository compromisoRepository,
        IOfrendaEspecialRepository ofrendaEspecialRepository,
        IMiembroRepository miembroRepository,
        ITransaccionRepository transaccionRepository,
        IUnitOfWork unitOfWork)
    {
        _compromisoRepository = compromisoRepository;
        _ofrendaEspecialRepository = ofrendaEspecialRepository;
        _miembroRepository = miembroRepository;
        _transaccionRepository = transaccionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CompromisoOfrendaDto> Handle(RegistrarCompromisoCommand request, CancellationToken cancellationToken)
    {
        var ofrenda = await _ofrendaEspecialRepository.ObtenerPorIdAsync(request.OfrendaEspecialId, cancellationToken)
            ?? throw new DomainException($"No existe la campaña #{request.OfrendaEspecialId}.");

        var miembro = await _miembroRepository.ObtenerPorIdAsync(request.MiembroId, cancellationToken)
            ?? throw new DomainException($"No existe el miembro #{request.MiembroId}.");

        var existente = await _compromisoRepository.ObtenerPorOfrendaYMiembroAsync(
            request.OfrendaEspecialId, request.MiembroId, cancellationToken);
        if (existente is not null)
            throw new DomainException(
                $"{miembro.NombreCompleto} ya tiene un compromiso registrado para '{ofrenda.Nombre}'. Edítalo en vez de crear uno nuevo.");

        var compromiso = new CompromisoOfrenda(request.OfrendaEspecialId, request.MiembroId, request.MontoComprometido);
        await _compromisoRepository.AgregarAsync(compromiso, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        // Por si el miembro ya le había dado a esta campaña antes de que se registrara
        // el compromiso (ej. se formaliza después de un primer aporte).
        var aportes = await _transaccionRepository.ObtenerAportesPorMiembroEnCampanaAsync(request.OfrendaEspecialId, cancellationToken);
        var montoDado = aportes.FirstOrDefault(a => a.MiembroId == request.MiembroId)?.Total ?? 0m;
        var estado = EstadoCompromisoCalculador.Calcular(montoDado, compromiso.MontoComprometido, ofrenda.FechaFin);

        return new CompromisoOfrendaDto(
            compromiso.Id, ofrenda.Id, miembro.Id, miembro.NombreCompleto,
            compromiso.MontoComprometido, montoDado, estado);
    }
}
