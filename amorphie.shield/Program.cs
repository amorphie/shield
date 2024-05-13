using System.Text.Json.Serialization;
using amorphie.core.Extension;
using amorphie.core.Identity;
using amorphie.core.Swagger;
using amorphie.shield;
using amorphie.shield.Swagger;
using amorphie.shield.Validator;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Prometheus;
using Swashbuckle.AspNetCore.SwaggerGen;
using amorphie.shield.ExceptionHandling;
using amorphie.shield.data;
using amorphie.shield.Module;

var builder = WebApplication.CreateBuilder(args);
// await builder.Configuration.AddVaultSecrets("amorphie-secretstore", new string[] { "amorphie-shield" });
// var postgreSql = builder.Configuration["shielddb"];

var postgreSql = "Host=localhost:5432;Database=shieldDb;Username=postgres;Password=postgres;Include Error Detail=true;";

builder.Services.AddApiVersioning(options =>
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
builder.Services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

// ---------------------------------------------

// If you use AutoInclude in context you should add  ReferenceHandler.IgnoreCycles to avoid circular load
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.WriteIndented = true;
});

//builder.Services.AddDaprClient();
//builder.Services.AddHealthChecks().AddBBTHealthCheck();

builder.Services.AddScoped<IBBTIdentity, FakeIdentity>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<AddSwaggerParameterFilter>();
    // c.OperationFilter<ApiVersionOperationFilter>();
});

builder.Services.AddValidatorsFromAssemblyContaining<CertificateValidator>(includeInternalTypes: true);
//builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddDbContext<ShieldDbContext>
    (options => options.UseNpgsql(postgreSql, b => b.MigrationsAssembly("amorphie.shield.data")));

builder.Services.AddDataServices();
builder.Services.AddManagerServices();
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ShieldDbContext>();

db.Database.Migrate();
DbInitializer.Initialize(db);

app.UseRouting();
// Configure the HTTP request pipeline.
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

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.AddRoutes();
app.MapCertificateModuleEndpoints();
app.RegisterTransactionEndpoints();

//app.MapHealthChecks("/healthz", new HealthCheckOptions
//{
//    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//});
app.MapMetrics();

app.Run();
