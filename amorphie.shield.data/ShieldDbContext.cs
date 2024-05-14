using System.Runtime.ConstrainedExecution;
using amorphie.shield.Certificates;
using amorphie.shield.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

namespace amorphie.shield;

class ShieldDbContextFactory : IDesignTimeDbContextFactory<ShieldDbContext>
{
    //lazy loading true
    //lazy loading false, eğer alt bileşenleri getirmek istiyorsak include kullanmamız lazım,eager loading
    private readonly IConfiguration _configuration;

    public ShieldDbContextFactory() { }

    public ShieldDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ShieldDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ShieldDbContext>();

        var connStr = "Host=localhost:5432;Database=shieldDb;Username=postgres;Password=postgres";
        builder.UseNpgsql(connStr);
        builder.EnableSensitiveDataLogging();
        return new ShieldDbContext(builder.Options);
    }
}

public class ShieldDbContext : DbContext
{
    public ShieldDbContext(DbContextOptions<ShieldDbContext> options)
        : base(options)
    {
        ChangeTracker.AutoDetectChangesEnabled = true;
    }

    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShieldDbContext).Assembly);

        modelBuilder.Entity<Certificate>(b =>
        {
            b.ToTable("Certificates");
            b.HasKey(p => p.Id);
            b.Property(p => p.Status).HasConversion(new EnumToNumberConverter<CertificateStatus, int>());
            b.OwnsOne(p => p.Identity, i =>
            {
                i.WithOwner();
                i.Property(ip => ip.DeviceId).HasColumnName("DeviceId");
                i.Property(ip => ip.RequestId).HasColumnName("RequestId");
                i.Property(ip => ip.TokenId).HasColumnName("TokenId");
                i.Property(ip => ip.UserTCKN).HasColumnName("UserTCKN").HasMaxLength(11);
            });
        });

        modelBuilder.Entity<Transaction>(b =>
        {
            b.ToTable("Transactions");
            b.HasKey(p => p.Id);
            b.Property(p => p.Status).HasConversion(new EnumToNumberConverter<TransactionStatus, int>());
            b.HasOne<Certificate>().WithMany().HasForeignKey(p => p.CertificateId).OnDelete(DeleteBehavior.Cascade);

            b.OwnsMany(o => o.Activities, i =>
            {
                i.ToTable("TransactionActivities");
                i.WithOwner(); 

                i.Property(p => p.Status).HasConversion(new EnumToNumberConverter<TransactionStatus, int>());
                i.HasOne<Transaction>().WithMany(p => p.Activities).HasForeignKey(p => p.TransactionId).OnDelete(DeleteBehavior.Cascade);
            });
        });
    }
}
