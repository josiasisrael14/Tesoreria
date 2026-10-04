using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.OfrendasEspeciales.Comandos.CrearOfrendaEspecial;

public class CrearOfrendaEspecialCommandHandler : IRequestHandler<CrearOfrendaEspecialCommand, OfrendaEspecialDto>
{
    private readonly IOfrendaEspecialRepository _ofrendaEspecialRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearOfrendaEspecialCommandHandler(
        IOfrendaEspecialRepository ofrendaEspecialRepository, IUnitOfWork unitOfWork)
    {
        _ofrendaEspecialRepository = ofrendaEspecialRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OfrendaEspecialDto> Handle(CrearOfrendaEspecialCommand request, CancellationToken cancellationToken)
    {
        if (await _ofrendaEspecialRepository.ExisteConNombreYAnioAsync(request.Nombre, request.Anio, cancellationToken))
            throw new DomainException($"Ya existe una campaña '{request.Nombre}' para el año {request.Anio}.");

        var ofrendaEspecial = new OfrendaEspecial(
            request.Nombre, request.Anio, request.Descripcion, request.FechaInicio, request.FechaFin, request.MetaMonto);

        await _ofrendaEspecialRepository.AgregarAsync(ofrendaEspecial, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new OfrendaEspecialDto(
            ofrendaEspecial.Id, ofrendaEspecial.Nombre, ofrendaEspecial.Anio, ofrendaEspecial.Descripcion,
            ofrendaEspecial.FechaInicio, ofrendaEspecial.FechaFin, ofrendaEspecial.MetaMonto, ofrendaEspecial.Activo);
    }
}
