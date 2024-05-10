using amorphie.shield.core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace amorphie.shield.data.Configurations;
public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.UserTCKN).IsRequired(false).HasMaxLength(11);

    }
}
