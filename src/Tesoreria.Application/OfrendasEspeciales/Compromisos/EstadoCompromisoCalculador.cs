namespace Tesoreria.Application.OfrendasEspeciales.Compromisos;

/// <summary>
/// Decide si un compromiso está Cumplido, Pendiente o EnMora. Igual que un
/// saldo, no se guarda: se recalcula cada vez que se pide, comparando lo dado
/// contra lo comprometido y contra la fecha límite de la campaña (su FechaFin —
/// no hay una fecha aparte para esto, ver CompromisoOfrenda).
/// </summary>
public static class EstadoCompromisoCalculador
{
    public const string Cumplido = "Cumplido";
    public const string Pendiente = "Pendiente";
    public const string EnMora = "EnMora";

    public static string Calcular(decimal montoDado, decimal montoComprometido, DateTime? fechaLimite)
    {
        if (montoDado >= montoComprometido)
            return Cumplido;

        if (fechaLimite.HasValue && DateTime.UtcNow.Date > fechaLimite.Value.Date)
            return EnMora;

        return Pendiente;
    }
}
