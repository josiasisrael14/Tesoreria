using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Comandos.DesactivarUsuario;

/// <summary>UsuarioActualId es quién está haciendo la petición, para poder bloquear que alguien se desactive a sí mismo.</summary>
public record DesactivarUsuarioCommand(string UsuarioId, string UsuarioActualId) : IRequest<bool>;
