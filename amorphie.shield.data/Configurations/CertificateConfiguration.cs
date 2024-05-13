using amorphie.shield.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace amorphie.shield.Configurations;
public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Status).HasConversion(new EnumToNumberConverter<CertificateStatus,int>());
    }
}
