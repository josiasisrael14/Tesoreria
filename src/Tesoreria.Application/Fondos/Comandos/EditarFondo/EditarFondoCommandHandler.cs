using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Fondos.Comandos.EditarFondo;

public class EditarFondoCommandHandler : IRequestHandler<EditarFondoCommand, FondoDto>
{
    private readonly IFondoRepository _fondoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditarFondoCommandHandler(IFondoRepository fondoRepository, IUnitOfWork unitOfWork)
    {
        _fondoRepository = fondoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FondoDto> Handle(EditarFondoCommand request, CancellationToken cancellationToken)
    {
        var fondo = await _fondoRepository.ObtenerPorIdAsync(request.FondoId, cancellationToken)
            ?? throw new DomainException($"No existe el fondo #{request.FondoId}.");

        // Solo se rechaza el nombre repetido si pertenece a OTRO fondo; si la
        // edición no le cambia el nombre (o lo cambia solo de mayúsculas), esta
        // comparación no debe bloquear el guardado.
        var nombreYaUsado = await _fondoRepository.ExisteConNombreAsync(request.Nombre, cancellationToken);
        if (nombreYaUsado && !string.Equals(fondo.Nombre, request.Nombre, StringComparison.OrdinalIgnoreCase))
            throw new DomainException($"Ya existe un fondo llamado '{request.Nombre}'.");

        // Solo un fondo puede ser "el" de Ofrendas Especiales: si este se marca,
        // se desmarca automáticamente el que lo era antes (si era otro distinto).
        if (request.EsFondoDeOfrendasEspeciales && !fondo.EsFondoDeOfrendasEspeciales)
        {
            var actual = await _fondoRepository.ObtenerFondoDeOfrendasEspecialesAsync(cancellationToken);
            if (actual is not null && actual.Id != fondo.Id)
                actual.DesmarcarComoFondoDeOfrendasEspeciales();
        }

        fondo.Editar(request.Nombre, request.Tipo, request.Descripcion, request.EsFondoDeOfrendasEspeciales);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new FondoDto(fondo.Id, fondo.Nombre, fondo.Descripcion, fondo.Tipo, fondo.Activo, fondo.EsFondoDeOfrendasEspeciales);
    }
}
