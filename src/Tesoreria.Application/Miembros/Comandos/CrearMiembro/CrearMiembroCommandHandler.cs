using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Entidades;

namespace Tesoreria.Application.Miembros.Comandos.CrearMiembro;

public class CrearMiembroCommandHandler : IRequestHandler<CrearMiembroCommand, MiembroDto>
{
    private readonly IMiembroRepository _miembroRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearMiembroCommandHandler(IMiembroRepository miembroRepository, IUnitOfWork unitOfWork)
    {
        _miembroRepository = miembroRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MiembroDto> Handle(CrearMiembroCommand request, CancellationToken cancellationToken)
    {
        var miembro = new Miembro(request.Nombres, request.Apellidos, request.DocumentoIdentidad,
            request.Telefono, request.Email);

        await _miembroRepository.AgregarAsync(miembro, cancellationToken);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new MiembroDto(miembro.Id, miembro.Nombres, miembro.Apellidos,
            miembro.DocumentoIdentidad, miembro.Telefono, miembro.Email, miembro.Activo);
    }
}
