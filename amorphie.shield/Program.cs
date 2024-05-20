using amorphie.core.Extension;
using amorphie.shield.Extensions;
using Prometheus;
namespace amorphie.shield;
public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.RegisterDbContext(builder.Configuration);
        builder.Services.RegisterApiVersioning();
        builder.Services.RegisterSwagger();
        builder.Services.RegisterShieldCore();
        builder.Services.RegisterServices();
        builder.Services.RegisterExceptionHandling();

        var app = builder.Build();
        app.UseDbMigrate();
        app.UseRouting();
        app.UseSwaggerMiddleware();
        app.UseHttpsRedirection();
        app.UseExceptionHandler();
        app.AddRoutes();
        app.AddModuleEndpoints();
        //app.MapHealthChecks("/healthz", new HealthCheckOptions
        //{
        //    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        //});
        app.MapMetrics();

        app.Run();
    }
}