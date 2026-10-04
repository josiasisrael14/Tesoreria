using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Fondos.Comandos.CrearFondo;

public class CrearFondoCommandHandler : IRequestHandler<CrearFondoCommand, FondoDto>
{
    private readonly IFondoRepository _fondoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearFondoCommandHandler(IFondoRepository fondoRepository, IUnitOfWork unitOfWork)
    {
        _fondoRepository = fondoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FondoDto> Handle(CrearFondoCommand request, CancellationToken cancellationToken)
    {
        if (await _fondoRepository.ExisteConNombreAsync(request.Nombre, cancellationToken))
            throw new DomainException($"Ya existe un fondo llamado '{request.Nombre}'.");

        // Solo un fondo puede ser "el" de Ofrendas Especiales: si se marca este,
        // se desmarca automáticamente el que lo era antes (si había uno).
        if (request.EsFondoDeOfrendasEspeciales)
        {
            var actual = await _fondoRepository.ObtenerFondoDeOfrendasEspecialesAsync(cancellationToken);
            actual?.DesmarcarComoFondoDeOfrendasEspeciales();
        }

        var fondo = new Fondo(request.Nombre, request.Tipo, request.Descripcion, request.EsFondoDeOfrendasEspeciales);

        await _fondoRepository.AgregarAsync(fondo, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new FondoDto(fondo.Id, fondo.Nombre, fondo.Descripcion, fondo.Tipo, fondo.Activo, fondo.EsFondoDeOfrendasEspeciales);
    }
}
