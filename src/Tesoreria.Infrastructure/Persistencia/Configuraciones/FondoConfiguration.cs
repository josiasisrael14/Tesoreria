using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tesoreria.Domain.Entidades;

namespace Tesoreria.Infrastructure.Persistencia.Configuraciones;

public class FondoConfiguration : IEntityTypeConfiguration<Fondo>
{
    public void Configure(EntityTypeBuilder<Fondo> builder)
    {
        builder.ToTable("Fondos");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(f => f.Nombre).IsUnique();

        builder.Property(f => f.Descripcion).HasMaxLength(400);

        builder.Property(f => f.Tipo).HasConversion<string>().HasMaxLength(20);
    }
}
