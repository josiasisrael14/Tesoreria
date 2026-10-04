namespace Tesoreria.Application.Miembros.Consultas.HistorialAportesMiembro;

public record AporteFondoDto(int FondoId, string NombreFondo, decimal Total);

public record AporteOfrendaEspecialDto(int OfrendaEspecialId, string NombreOfrendaEspecial, decimal Total);

/// <summary>
/// Lo que ha dado un miembro, desglosado por fondo (diezmos, ofrenda general, etc.)
/// y, dentro de Ofrendas Especiales, por campaña (misionera, pro templo, aniversario...).
/// Los montos son netos: si una transacción fue reversada, ya no cuenta.
/// </summary>
public record HistorialAportesMiembroDto(
    int MiembroId,
    string NombreMiembro,
    List<AporteFondoDto> AportesPorFondo,
    List<AporteOfrendaEspecialDto> AportesPorOfrendaEspecial)
{
    public decimal TotalGeneral => AportesPorFondo.Sum(a => a.Total);
}
