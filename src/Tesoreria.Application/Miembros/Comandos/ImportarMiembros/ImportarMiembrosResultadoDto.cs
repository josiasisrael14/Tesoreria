namespace Tesoreria.Application.Miembros.Comandos.ImportarMiembros;

/// <summary>Una fila del archivo que no se pudo importar, y por qué.</summary>
public record ImportarMiembroErrorDto(int Fila, string Mensaje);

/// <summary>Resumen de la importación: cuántas filas traía el archivo, cuántas se
/// crearon y el detalle de las que fallaron (para mostrarle al usuario qué corregir).</summary>
public record ImportarMiembrosResultadoDto(
    int TotalFilas,
    int Creados,
    List<ImportarMiembroErrorDto> Errores);
