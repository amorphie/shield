using System.Text.Json.Serialization;
using amorphie.core.Identity;
using amorphie.core.Swagger;
using amorphie.shield.ExceptionHandling;
using amorphie.shield.Swagger;
using amorphie.shield.Validator;
using amorphie.shield.HealthChecks;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace amorphie.shield.Extensions;

public static class ServiceCollectionExtensions
{
    private static IServiceCollection RegisterDbContext(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // await builder.Configuration.AddVaultSecrets("amorphie-secretstore", new string[] { "amorphie-shield" });
        var postgreSql = builderConfiguration["shielddb"];
        //TODO: Remove
        //var postgreSql =
        //    "Host=localhost:5432;Database=shieldDb;Username=postgres;Password=postgres;Include Error Detail=true;";

        services.AddDbContext<ShieldDbContext>
            (options => {
                options.UseNpgsql(postgreSql, b => {
                    b.MigrationsAssembly("amorphie.shield.data");
                    b.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    });
                });
        return services;
    }

    private static IServiceCollection RegisterApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new HeaderApiVersionReader("X-Api-Version"),
                new QueryStringApiVersionReader("v"),
                new UrlSegmentApiVersionReader());
        }).AddApiExplorer(
            setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            }
        );

        return services;
    }

    private static IServiceCollection RegisterSwagger(this IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<AddSwaggerParameterFilter>();
            // c.OperationFilter<ApiVersionOperationFilter>();
        });

        return services;
    }

    public static IServiceCollection RegisterShieldCore(this IServiceCollection services, ConfigurationManager configuration)
    {
        // If you use AutoInclude in context you should add  ReferenceHandler.IgnoreCycles to avoid circular load
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.SerializerOptions.WriteIndented = true;
        });
        services.AddDaprClient();
        services.RegisterDbContext();
        services.RegisterApiVersioning();
        services.RegisterSwagger();
        services.RegisterServices(configuration);
        services.RegisterExceptionHandling();
        services.RegisterHealthCheck();

        services.AddScoped<IBBTIdentity, FakeIdentity>();
        services.AddValidatorsFromAssemblyContaining<CertificateValidator>(includeInternalTypes: true);
        //builder.Services.AddAutoMapper(typeof(Program).Assembly);
        return services;
    }

    private static IServiceCollection RegisterHealthCheck(this IServiceCollection services)
    {
        // var postgreSql = builder.Configuration["shielddb"];
        //TODO: Remove
        var postgreSql =
            "Host=localhost:5432;Database=shieldDb;Username=postgres;Password=postgres;Include Error Detail=true;";
        
        services.AddHealthChecks()
            .AddNpgSql(postgreSql, tags: new[] { "PostgresDb" })
            .AddDapr(tags: new[] { "Dapr" })
            .AddDaprSecretStore(tags: new[] { "DaprSecretStore" });

        return services;
    }

    private static IServiceCollection RegisterExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<ExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }
    
    public static IServiceCollection RegisterServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDataServices();
        services.AddManagerServices(configuration);
        return services;
    }    

}
