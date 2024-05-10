using amorphie.shield.core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace amorphie.shield.data.Configurations;
public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Status).HasConversion(new EnumToStringConverter<CertificateStatus>());
    }
}
