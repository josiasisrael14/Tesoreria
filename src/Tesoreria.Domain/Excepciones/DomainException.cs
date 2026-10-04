namespace Tesoreria.Domain.Excepciones;

/// <summary>
/// Se lanza cuando se viola una regla de negocio del dominio de tesorería
/// (por ejemplo: un monto inválido, o reversar una transacción ya reversada).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
