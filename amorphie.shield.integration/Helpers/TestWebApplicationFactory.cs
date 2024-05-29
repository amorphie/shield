using amorphie.shield.CertManager;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace amorphie.shield.test.integration;

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ShieldDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            var vaultDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICaManager));

            if (vaultDescriptor != null)
            {
                services.Remove(vaultDescriptor);
                services.AddSingleton<ICaManager, FileCaManager>();

            }

            services.AddDbContext<ShieldDbContext>(options =>
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                options.UseSqlite($"Data Source={Path.Join(path, "shield_tests.db")}");
            });

        });
        return base.CreateHost(builder);
    }
}
