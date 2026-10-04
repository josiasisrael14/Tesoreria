using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tesoreria.Domain.Entidades;

namespace Tesoreria.Infrastructure.Persistencia.Configuraciones;

public class OfrendaEspecialConfiguration : IEntityTypeConfiguration<OfrendaEspecial>
{
    public void Configure(EntityTypeBuilder<OfrendaEspecial> builder)
    {
        builder.ToTable("OfrendasEspeciales");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(o => o.Anio).IsRequired();

        // Antes el nombre era único por sí solo. Ahora se permite repetir el mismo
        // nombre de campaña en años distintos (ej. "Aniversario" en 2026 y 2027),
        // así que la combinación Nombre + Año es lo que tiene que ser única.
        builder.HasIndex(o => new { o.Nombre, o.Anio }).IsUnique();

        builder.Property(o => o.Descripcion).HasMaxLength(400);
        builder.Property(o => o.MetaMonto).HasColumnType("decimal(18,2)");
    }
}
