using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tesoreria.Domain.Entidades;

namespace Tesoreria.Infrastructure.Persistencia.Configuraciones;

public class CultoConfiguration : IEntityTypeConfiguration<Culto>
{
    public void Configure(EntityTypeBuilder<Culto> builder)
    {
        builder.ToTable("Cultos");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Tipo).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Descripcion).HasMaxLength(400);
    }
}
