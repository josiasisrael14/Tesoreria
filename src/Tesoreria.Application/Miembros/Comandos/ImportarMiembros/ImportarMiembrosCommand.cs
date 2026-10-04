using Tesoreria.Application.Abstracciones.Mediator;

namespace Tesoreria.Application.Miembros.Comandos.ImportarMiembros;

/// <summary>Una fila del Excel que se está importando. "Fila" es el número de fila original
/// (tal como aparece en el archivo), solo para poder señalar errores de forma clara al usuario.</summary>
public record ImportarMiembroItem(
    int Fila,
    string Nombres,
    string Apellidos,
    string? DocumentoIdentidad,
    string? Telefono,
    string? Email);

/// <summary>
/// Crea muchos miembros de una sola vez a partir de un archivo Excel que el usuario
/// subió (el parseo del archivo lo hace el frontend; acá solo llegan los datos ya
/// separados por fila). No usa el ValidationBehavior normal porque, a diferencia de
/// CrearMiembroCommand, una fila inválida no debe frenar a las demás: se crean las
/// filas válidas y se informan los errores de las que no lo son.
/// </summary>
public record ImportarMiembrosCommand(List<ImportarMiembroItem> Items) : IRequest<ImportarMiembrosResultadoDto>;
