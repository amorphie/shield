using amorphie.shield.core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace amorphie.shield.data.Configurations;
public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Status).HasConversion(new EnumToStringConverter<TransactionStatus>());
        builder.HasOne<Certificate>().WithMany().HasForeignKey(p => p.CertificateId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class TransactionActivityConfiguration : IEntityTypeConfiguration<TransactionActivity>
{
    public void Configure(EntityTypeBuilder<TransactionActivity> builder)
    {
        builder.ToTable("TransactionActivities");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Status).HasConversion(new EnumToStringConverter<TransactionStatus>());
        builder.HasOne<Transaction>().WithMany(p => p.Activities).HasForeignKey(p => p.TransactionId).OnDelete(DeleteBehavior.Cascade);
    }
}

