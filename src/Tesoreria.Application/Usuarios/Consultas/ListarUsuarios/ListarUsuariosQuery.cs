using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Consultas.ListarUsuarios;

public record ListarUsuariosQuery : IRequest<List<UsuarioDto>>;
