using Tesoreria.Application.Abstracciones.Identidad;
using Tesoreria.Application.Abstracciones.Mediator;
using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Application.Usuarios.Comandos.CrearUsuario;

public class CrearUsuarioCommandHandler : IRequestHandler<CrearUsuarioCommand, UsuarioDto>
{
    private readonly IIdentityService _identityService;

    public CrearUsuarioCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<UsuarioDto> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
    {
        var resultado = await _identityService.CrearUsuarioAsync(
            request.Email, request.NombreCompleto, request.Contrasena, request.Rol, cancellationToken);

        if (!resultado.Exitoso)
            throw new DomainException(string.Join(" ", resultado.Errores));

        return resultado.Usuario!;
    }
}
