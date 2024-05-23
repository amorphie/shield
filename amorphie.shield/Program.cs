using amorphie.core.Extension;
using amorphie.shield.Extensions;
using Dapr.Client;
using Prometheus;
using static amorphie.core.Extension.VaultConfigExtension;

namespace amorphie.shield;
public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        await builder.Configuration.AddVaultSecrets("shield-secretstore", new[] { "shield-secretstore" });

        builder.Services.RegisterDbContext(builder.Configuration);
        builder.Services.RegisterApiVersioning();
        builder.Services.RegisterSwagger();
        builder.Services.RegisterShieldCore();
        builder.Services.RegisterServices();
        builder.Services.RegisterExceptionHandling();
        builder.Services.AddHealthChecks();
        var app = builder.Build();
        app.UseDbMigrate();
        app.UseRouting();
        app.UseSwaggerMiddleware();
        app.UseHttpsRedirection();
        app.UseExceptionHandler();
        app.AddRoutes();
        app.AddModuleEndpoints();
        app.MapHealthChecks("/health");

        app.MapMetrics();

        app.Run();
    }
}