using amorphie.shield.Certificates;
using amorphie.shield.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace amorphie.shield;

public static class ServicesCollectionExtension
{
    public static void AddDataServices(this IServiceCollection services)
    {
        services.AddScoped<CertificateRepository>();
        services.AddScoped<TransactionRepository>();
    }
}
