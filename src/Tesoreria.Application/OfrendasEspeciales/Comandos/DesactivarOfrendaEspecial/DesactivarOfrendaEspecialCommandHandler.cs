using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.OfrendasEspeciales.Comandos.DesactivarOfrendaEspecial;

public class DesactivarOfrendaEspecialCommandHandler : IRequestHandler<DesactivarOfrendaEspecialCommand, bool>
{
    private readonly IOfrendaEspecialRepository _ofrendaEspecialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DesactivarOfrendaEspecialCommandHandler(
        IOfrendaEspecialRepository ofrendaEspecialRepository, IUnitOfWork unitOfWork)
    {
        _ofrendaEspecialRepository = ofrendaEspecialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DesactivarOfrendaEspecialCommand request, CancellationToken cancellationToken)
    {
        var ofrendaEspecial = await _ofrendaEspecialRepository.ObtenerPorIdAsync(request.OfrendaEspecialId, cancellationToken)
            ?? throw new DomainException($"No existe la ofrenda especial #{request.OfrendaEspecialId}.");

        ofrendaEspecial.Desactivar();
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return true;
    }
}
