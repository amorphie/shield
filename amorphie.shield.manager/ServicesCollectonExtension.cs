using amorphie.shield.app.Db;
using Microsoft.Extensions.DependencyInjection;
using amorphie.shield.app.CertManager;

namespace amorphie.shield.app;
public static class ServicesCollectonExtension
{
    public static void AddManagerServices(this IServiceCollection services)
    {

        services.AddScoped<CertificateService>();
        services.AddScoped<CertificateManager>();
    }
}

