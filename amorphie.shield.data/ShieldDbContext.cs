using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using amorphie.shield.core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace amorphie.shield.data;

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
        // var test = _configuration["STATE_STORE"];
        // System.Console.WriteLine("Test: " + test);


        var connStr = "Host=localhost:5432;Database=shieldDb;Username=postgres;Password=postgres";
        builder.UseNpgsql(connStr);
        builder.EnableSensitiveDataLogging();
        return new ShieldDbContext(builder.Options);
    }
}

public class ShieldDbContext : DbContext
{
    public ShieldDbContext(DbContextOptions<ShieldDbContext> options)
        : base(options) { }

    public DbSet<Certificate> Certificates { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}
