namespace Tesoreria.Domain.Entidades;

/// <summary>
/// Campos comunes a toda entidad persistida: identidad y auditoría básica.
/// </summary>
public abstract class EntidadBase
{
    public int Id { get; protected set; }
    public DateTime FechaCreacion { get; protected set; } = DateTime.UtcNow;
}
