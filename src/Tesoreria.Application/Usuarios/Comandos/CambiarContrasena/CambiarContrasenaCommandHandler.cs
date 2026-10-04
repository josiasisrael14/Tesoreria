using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Usuarios.Comandos.CambiarContrasena;

public class CambiarContrasenaCommandHandler : IRequestHandler<CambiarContrasenaCommand, bool>
{
    private readonly IIdentityService _identityService;

    public CambiarContrasenaCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(CambiarContrasenaCommand request, CancellationToken cancellationToken)
    {
        var resultado = await _identityService.CambiarContrasenaAsync(
            request.UsuarioId, request.ContrasenaActual, request.ContrasenaNueva, cancellationToken);

        if (!resultado.Exitoso)
            throw new DomainException(string.Join(" ", resultado.Errores));

        return true;
    }
}
