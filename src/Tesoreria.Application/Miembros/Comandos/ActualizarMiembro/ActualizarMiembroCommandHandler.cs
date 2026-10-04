using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Application.Abstracciones.Persistencia;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Miembros.Comandos.ActualizarMiembro;

public class ActualizarMiembroCommandHandler : IRequestHandler<ActualizarMiembroCommand, MiembroDto>
{
    private readonly IMiembroRepository _miembroRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarMiembroCommandHandler(IMiembroRepository miembroRepository, IUnitOfWork unitOfWork)
    {
        _miembroRepository = miembroRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MiembroDto> Handle(ActualizarMiembroCommand request, CancellationToken cancellationToken)
    {
        var miembro = await _miembroRepository.ObtenerPorIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException($"No existe el miembro #{request.Id}.");

        miembro.ActualizarDatos(request.Nombres, request.Apellidos, request.DocumentoIdentidad,
            request.Telefono, request.Email);

        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new MiembroDto(miembro.Id, miembro.Nombres, miembro.Apellidos,
            miembro.DocumentoIdentidad, miembro.Telefono, miembro.Email, miembro.Activo);
    }
}
