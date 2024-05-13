using Microsoft.Extensions.DependencyInjection;
using amorphie.shield.Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.Transactions;

namespace amorphie.shield;
public static class ServicesCollectionExtension
{
    public static void AddManagerServices(this IServiceCollection services)
    {

        services.AddScoped<ICertificateAppService, CertificateAppService>();
        services.AddScoped<CaManager>();
        services.AddScoped<CertificateManager>();
        services.AddScoped<ITransactionAppService, TransactionAppService>();
    }
}

