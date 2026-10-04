using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Usuarios.Comandos.ActualizarFoto;

public class ActualizarFotoCommandHandler : IRequestHandler<ActualizarFotoCommand, UsuarioDto>
{
    private readonly IIdentityService _identityService;

    public ActualizarFotoCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<UsuarioDto> Handle(ActualizarFotoCommand request, CancellationToken cancellationToken)
    {
        var resultado = await _identityService.ActualizarFotoAsync(request.UsuarioId, request.FotoUrl, cancellationToken);

        if (!resultado.Exitoso)
            throw new DomainException(string.Join(" ", resultado.Errores));

        return resultado.Usuario!;
    }
}
