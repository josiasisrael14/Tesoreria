using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Fondos.Comandos.DesactivarFondo;

public class DesactivarFondoCommandHandler : IRequestHandler<DesactivarFondoCommand, bool>
{
    private readonly IFondoRepository _fondoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DesactivarFondoCommandHandler(IFondoRepository fondoRepository, IUnitOfWork unitOfWork)
    {
        _fondoRepository = fondoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DesactivarFondoCommand request, CancellationToken cancellationToken)
    {
        var fondo = await _fondoRepository.ObtenerPorIdAsync(request.FondoId, cancellationToken)
            ?? throw new DomainException($"No existe el fondo #{request.FondoId}.");

        fondo.Desactivar();
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return true;
    }
}
