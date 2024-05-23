using Microsoft.Extensions.DependencyInjection;
using amorphie.shield.Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.Transactions;
using amorphie.shield.Revokes;

namespace amorphie.shield;
public static class ServicesCollectionExtension
{
    public static void AddManagerServices(this IServiceCollection services)
    {

        services.AddScoped<ICertificateAppService, CertificateAppService>();
        services.AddScoped<IRevokeAppService, RevokeAppService>();
        services.AddSingleton<ICaManager, VaultCaManager>();
        services.AddScoped<CertificateManager>();
        services.AddScoped<ITransactionAppService, TransactionAppService>();
    }
}

