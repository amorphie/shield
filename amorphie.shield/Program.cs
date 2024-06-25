using amorphie.core.Extension;
using amorphie.shield.Extensions;
using Prometheus;

namespace amorphie.shield;
public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        await builder.Configuration.AddVaultSecrets("shield-secretstore", new[] { "shield-secretstore" });
        builder.RegisterOptions();

        builder.Services.RegisterShieldCore(builder.Configuration);
        var app = builder.Build();
        app.UseDbMigrate();
        app.UseRouting();
        app.UseSwaggerMiddleware();
        app.UseHttpsRedirection();
        app.UseExceptionHandler();
        app.AddRoutes();
        app.AddModuleEndpoints();
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions(){
            Predicate = _ => true,
            AllowCachingResponses = false,
            ResponseWriter = HealthCheckOptionsExtensions.WriteResponse
        });

        app.MapMetrics();

        await app.RunAsync();
    }
}
