using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.OfrendasEspeciales.Compromisos.Comandos.EliminarCompromiso;

public class EliminarCompromisoCommandHandler : IRequestHandler<EliminarCompromisoCommand, bool>
{
    private readonly ICompromisoOfrendaRepository _compromisoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarCompromisoCommandHandler(ICompromisoOfrendaRepository compromisoRepository, IUnitOfWork unitOfWork)
    {
        _compromisoRepository = compromisoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(EliminarCompromisoCommand request, CancellationToken cancellationToken)
    {
        var compromiso = await _compromisoRepository.ObtenerPorIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException($"No existe el compromiso #{request.Id}.");

        _compromisoRepository.Eliminar(compromiso);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return true;
    }
}
