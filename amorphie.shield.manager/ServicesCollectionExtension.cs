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
        services.AddScoped<ICertificateManager, VaultCertificateManager>();
        services.AddScoped<ITransactionAppService, TransactionAppService>();
        services.AddScoped<IVaultClient, VaultClient>(sp => VaultClientCreator(configuration));
    }

    public static void RegisterOptions(this WebApplicationBuilder builder)
    {
        var vaultOptions = new VaultOptions();
        builder.Configuration.GetSection(VaultOptions.Vault).Bind(vaultOptions);
        vaultOptions.RoleName = builder.Configuration[VaultOptions.ROLE_NAME] ?? throw new KeyNotFoundException("Vault role name expected");
        //TODO: Remove these 5 lines
        var vaultTokenFileName = builder.Configuration["Vault:TokenFileName"] ?? throw new KeyNotFoundException("Vault token file name expected");// "admin";//Environment.GetEnvironmentVariable("VAULT_TOKEN");
        var vaultTokenFormFile = File.ReadAllText(vaultTokenFileName) ?? throw new KeyNotFoundException("Vault token expected");
        Console.WriteLine("Vault token file path: " + vaultTokenFileName);
        Console.WriteLine("Vault token : " + vaultTokenFormFile);
        Console.WriteLine("Vault role name : " + vaultOptions.RoleName);
        
        builder.Services.AddScoped(sp => vaultOptions);
    }
    public static VaultClient VaultClientCreator(ConfigurationManager configuration)
    {
        var vaultAddress = configuration[VaultOptions.VAULT_ADDR]; //; "http://127.0.0.1:8200";
        var vaultTokenFileName = configuration["Vault:TokenFileName"] ?? throw new KeyNotFoundException("Vault token file name expected");// "admin";//Environment.GetEnvironmentVariable("VAULT_TOKEN");
        var vaultTokenFormFile = File.ReadAllText(vaultTokenFileName) ?? throw new KeyNotFoundException("Vault token expected");
        Console.WriteLine("Vault token : " + vaultTokenFormFile);
        var authMethod = new TokenAuthMethodInfo(vaultTokenFormFile);
        var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod);
        return new VaultClient(vaultClientSettings);
    }
}



