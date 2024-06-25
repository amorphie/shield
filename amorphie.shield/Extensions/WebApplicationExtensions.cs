using amorphie.shield.Module;
using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
namespace amorphie.shield.Extensions;

public static class WebApplicationExtensions
{
    public static void UseDbMigrate(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShieldDbContext>();

        db.Database.Migrate();

        if(app.Environment.IsDevelopment()){
            DbInitializer.Initialize(db);
        }        
    }
    
    public static void UseSwaggerMiddleware(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            // Enable middleware to serve Swagger UI.
            app.UseSwaggerUI(o =>
            {

                var versionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in versionDescriptionProvider.ApiVersionDescriptions)
                {
                    o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        $"MyApi - {description.GroupName.ToUpper()}");
                }
                /*
                    var descriptions = app.DescribeApiVersions();

                    // build a swagger endpoint for each discovered API version
                    foreach (var description in descriptions)
                    {
                        var url = $"/swagger/{description.GroupName}/swagger.json";
                        var name = description.GroupName.ToUpperInvariant();
                        o.SwaggerEndpoint(url, name);
                    }
                    */
            });
        }
    }

    public static void AddModuleEndpoints(this WebApplication app)
    {
        app.RegisterCertificateModuleEndpoints();
        app.RegisterRevokeModuleEndpoints();
        app.RegisterTransactionModuleEndpoints();
    }

}
