using amorphie.shield.Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.Revokes;
using amorphie.shield.Transactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit.DependencyInjection.AspNetCoreTesting;


[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace amorphie.shield;

public class Startup
{
    public void ConfigureHost(IHostBuilder hostBuilder) =>
        hostBuilder.ConfigureWebHost(webHostBuilder => webHostBuilder
            .UseTestServerAndAddDefaultHttpClient()
            .UseStartup<AspNetCoreStartup>());

    public IHostBuilder CreateHostBuilder() => MinimalApiHostBuilderFactory.GetHostBuilder<Program>();

    private class AspNetCoreStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ShieldDbContext>
            (options =>
            {
                options.UseInMemoryDatabase("shieldDb");
            });

            services.AddSingleton<ICaManager, FileCaManager>();
            services.AddScoped<CertificateManager>();
            services.AddScoped<ITransactionAppService, TransactionAppService>();
            services.AddScoped<ICertificateAppService, CertificateAppService>();
            services.AddScoped<IRevokeAppService, RevokeAppService>();
            services.AddScoped<CertificateRepository>();
            services.AddScoped<TransactionRepository>();
        }

        public void Configure(IApplicationBuilder app) =>
            app.Run(static context => context.Response.WriteAsync(Guid.NewGuid().ToString("N")));
    }
}
