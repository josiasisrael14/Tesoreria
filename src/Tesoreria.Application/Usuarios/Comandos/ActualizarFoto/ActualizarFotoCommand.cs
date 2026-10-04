using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Usuarios.Comandos.ActualizarFoto;

/// <summary>Pasar FotoUrl=null quita la foto de perfil actual (volver al ícono con la inicial).</summary>
public record ActualizarFotoCommand(string UsuarioId, string? FotoUrl) : IRequest<UsuarioDto>;
