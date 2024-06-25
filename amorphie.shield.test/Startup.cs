using amorphie.shield.Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.Revokes;
using amorphie.shield.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp;

namespace amorphie.shield.test;

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
        services.AddScoped<ICertificateManager, CertificateManager>();
        services.AddScoped<ITransactionAppService, TransactionAppService>();
        services.AddScoped<ICertificateAppService, CertificateAppService>();
        services.AddScoped<IRevokeAppService, RevokeAppService>();
        services.AddScoped<CertificateRepository>();
        services.AddScoped<TransactionRepository>();

        services.Configure<VaultOptions>(opt =>
        {
            opt.CommonName = "burgan.com";
            opt.RoleName = "role_generate";
            opt.RSAKeySizeInBits = 2048;
            opt.TimeToLive = "3650d";
        });



        //TODO : D�zelt

        // Initialize Vault Client
        var vaultAddress = "http://127.0.0.1:8200";
        var vaultToken = "admin";//Environment.GetEnvironmentVariable("VAULT_TOKEN");

        var authMethod = new TokenAuthMethodInfo(vaultToken);
        var vaultClientSettings = new VaultClientSettings(vaultAddress, authMethod);
        var vaultClient = new VaultClient(vaultClientSettings);

        services.AddScoped<IVaultClient, VaultClient>(sp => vaultClient);
    }
}
