using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;

namespace Tesoreria.Application.Cultos.Comandos.CrearCulto;

public class CrearCultoCommandHandler : IRequestHandler<CrearCultoCommand, CultoDto>
{
    private readonly ICultoRepository _cultoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearCultoCommandHandler(ICultoRepository cultoRepository, IUnitOfWork unitOfWork)
    {
        _cultoRepository = cultoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CultoDto> Handle(CrearCultoCommand request, CancellationToken cancellationToken)
    {
        var culto = new Culto(request.Fecha, request.Tipo, request.Descripcion);

        await _cultoRepository.AgregarAsync(culto, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new CultoDto(culto.Id, culto.Fecha, culto.Tipo, culto.Descripcion);
    }
}
