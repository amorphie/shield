using amorphie.shield.Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace amorphie.shield;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ShieldDbContext>
            (options =>
            {
                options.UseInMemoryDatabase("shieldDb");
            });

        services.AddSingleton<ICaManager, FileCaManager>();
        services.AddScoped<CertificateManager>();
        services.AddScoped<ITransactionAppService, TransactionAppService>();
        services.AddScoped<ICertificateAppService, CertificateAppService>();
        services.AddScoped<CertificateRepository>();
        services.AddScoped<TransactionRepository>();
    }
}
