using Tesoreria.Domain.Excepciones;

namespace Tesoreria.Domain.Entidades;

public enum TipoCulto
{
    Dominical = 1,
    Misionero = 2,
    Especial = 3,
    Ayuno = 4,
    Otro = 5
}

/// <summary>
/// Un culto/servicio bajo el cual se agrupan las ofrendas recogidas ese día
/// (dominical, misionero, especial, etc.).
/// </summary>
public class Culto : EntidadBase
{
    public DateTime Fecha { get; private set; }
    public TipoCulto Tipo { get; private set; }
    public string? Descripcion { get; private set; }

    protected Culto() { } // EF Core

    public Culto(DateTime fecha, TipoCulto tipo, string? descripcion = null)
    {
        if (fecha == default)
            throw new DomainException("La fecha del culto es obligatoria.");

        Fecha = fecha.Date;
        Tipo = tipo;
        Descripcion = descripcion;
    }

    public void ActualizarDescripcion(string? descripcion) => Descripcion = descripcion;
}
