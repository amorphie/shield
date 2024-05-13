using System.Text.Json.Serialization;
using amorphie.core.Identity;
using amorphie.core.Swagger;
using amorphie.shield.ExceptionHandling;
using amorphie.shield.Swagger;
using amorphie.shield.Validator;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace amorphie.shield.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterDbContext(this IServiceCollection services,
        ConfigurationManager builderConfiguration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // await builder.Configuration.AddVaultSecrets("amorphie-secretstore", new string[] { "amorphie-shield" });
        // var postgreSql = builder.Configuration["shielddb"];
        //TODO: Remove
        var postgreSql =
            "Host=localhost:5432;Database=shieldDb;Username=postgres;Password=postgres;Include Error Detail=true;";

        services.AddDbContext<ShieldDbContext>
            (options => options.UseNpgsql(postgreSql, b => b.MigrationsAssembly("amorphie.shield.data")));
        return services;
    }

    public static IServiceCollection RegisterApiVersioning(this IServiceCollection services)
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

    public static IServiceCollection RegisterSwagger(this IServiceCollection services)
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

    public static IServiceCollection RegisterShieldCore(this IServiceCollection services)
    {
        // If you use AutoInclude in context you should add  ReferenceHandler.IgnoreCycles to avoid circular load
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.SerializerOptions.WriteIndented = true;
        });

        //builder.Services.AddDaprClient();
        //builder.Services.AddHealthChecks().AddBBTHealthCheck();

        services.AddScoped<IBBTIdentity, FakeIdentity>();
        services.AddValidatorsFromAssemblyContaining<CertificateValidator>(includeInternalTypes: true);
        //builder.Services.AddAutoMapper(typeof(Program).Assembly);
        return services;
    }

    public static IServiceCollection RegisterExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<ExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }
    
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddDataServices();
        services.AddManagerServices();
        return services;
    }
}