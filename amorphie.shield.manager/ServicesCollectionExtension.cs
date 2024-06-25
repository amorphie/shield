using Microsoft.Extensions.DependencyInjection;
using amorphie.shield.Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.Transactions;
using amorphie.shield.Revokes;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace amorphie.shield;
public static class ServicesCollectionExtension
{
    public static void AddManagerServices(this IServiceCollection services, ConfigurationManager configuration)
    {

        services.AddScoped<ICertificateAppService, CertificateAppService>();
        services.AddScoped<IRevokeAppService, RevokeAppService>();
        services.AddSingleton<ICaManager, VaultCaManager>();
        //services.AddScoped<ICertificateManager, CertificateManager>();
        services.AddScoped<ICertificateManager, VaultCertificateManager>();
        services.AddScoped<ITransactionAppService, TransactionAppService>();


        // Initialize Vault Client
        var vaultAddress = configuration["Vault:Address"]; //; "http://127.0.0.1:8200";
        var vaultToken = configuration["Vault:Token"];// "admin";//Environment.GetEnvironmentVariable("VAULT_TOKEN");

        var authMethod = new TokenAuthMethodInfo(vaultToken);
        var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod);
        var vaultClient = new VaultClient(vaultClientSettings);

        services.AddScoped<IVaultClient, VaultClient>(sp => vaultClient);
    }

    public static void RegisterOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<VaultOptions>(builder.Configuration.GetSection(VaultOptions.Vault));
    }
}

