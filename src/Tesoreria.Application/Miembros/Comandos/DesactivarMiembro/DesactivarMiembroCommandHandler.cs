using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Miembros.Comandos.DesactivarMiembro;

public class DesactivarMiembroCommandHandler : IRequestHandler<DesactivarMiembroCommand, bool>
{
    private readonly IMiembroRepository _miembroRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DesactivarMiembroCommandHandler(IMiembroRepository miembroRepository, IUnitOfWork unitOfWork)
    {
        _miembroRepository = miembroRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DesactivarMiembroCommand request, CancellationToken cancellationToken)
    {
        var miembro = await _miembroRepository.ObtenerPorIdAsync(request.MiembroId, cancellationToken)
            ?? throw new DomainException($"No existe el miembro #{request.MiembroId}.");

        miembro.Desactivar();
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return true;
    }
}
