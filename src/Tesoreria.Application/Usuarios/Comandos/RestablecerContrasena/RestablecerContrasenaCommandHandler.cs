using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Usuarios.Comandos.RestablecerContrasena;

public class RestablecerContrasenaCommandHandler : IRequestHandler<RestablecerContrasenaCommand, bool>
{
    private readonly IIdentityService _identityService;

    public RestablecerContrasenaCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(RestablecerContrasenaCommand request, CancellationToken cancellationToken)
    {
        var resultado = await _identityService.RestablecerContrasenaAsync(request.UsuarioId, request.NuevaContrasena, cancellationToken);

        if (!resultado.Exitoso)
            throw new DomainException(string.Join(" ", resultado.Errores));

        return true;
    }
}
