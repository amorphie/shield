using amorphie.shield.core.Model;
using amorphie.core.Module.minimal_api;
using amorphie.shield.data;
using Microsoft.AspNetCore.Mvc;
using amorphie.shield.core.Search;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using amorphie.core.Swagger;
using Microsoft.OpenApi.Models;
using amorphie.core.Identity;
using Asp.Versioning;
using Asp.Versioning.Builder;
using amorphie.core.Extension;

namespace amorphie.shield.Module;


public sealed class CertModule : BaseBBTRoute<CertificateDTO, Certificate, ShieldDbContext>
{
    public CertModule(WebApplication app)
        : base(app) { }

    public override string[]? PropertyCheckList => new string[] { "FirstMidName", "LastName" };

    public override string? UrlFragment => "cert";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {

        base.AddRoutes(routeGroupBuilder);

        routeGroupBuilder.MapGet("/search", SearchMethod);
        routeGroupBuilder.MapGet("/custom-method", CustomMethod);


        #region versioning

        ApiVersionSetBuilder apiVersionSetBuilder = new("Version1");
        apiVersionSetBuilder.HasApiVersion(new ApiVersion(1, 0));
        ApiVersionSet apiVersionSet = apiVersionSetBuilder.Build();


        routeGroupBuilder.MapGet("/v{version:apiVersion}/hello", () => "Hello from API URL version 1")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0));
        // [AddSwaggerParameterAttribute("X-Api-Version",ParameterLocation.Header,true)] 
        routeGroupBuilder.MapGet("/hello1", () => "Hello from API URL version 2")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(2, 0))
.HasApiVersion(new ApiVersion(1, 0));


        #endregion
        /*

                routeGroupBuilder.MapGet("v{version:apiVersion}/version1", (HttpContext context) =>
        {
            var apiVersion = context.GetRequestedApiVersion();

            return "Version " + apiVersion?.MajorVersion?.ToString();
        })
            .WithApiVersionSet( new Asp.Versioning.Builder.ApiVersionSetBuilder("Version1") {  new ApiVersion(1, 0) });
            .HasApiVersion(new ApiVersion(1, 0));
        */
    }
    protected override ValueTask<IResult> UpsertMethod([FromServices] IMapper mapper, [FromServices] FluentValidation.IValidator<Certificate> validator, [FromServices] ShieldDbContext context, [FromServices] IBBTIdentity bbtIdentity, [FromBody] CertificateDTO data, HttpContext httpContext, CancellationToken token)
    {
        return base.UpsertMethod(mapper, validator, context, bbtIdentity, data, httpContext, token);
    }
    [AddSwaggerParameter("Test Required", ParameterLocation.Header, true)]
    protected async ValueTask<IResult> CustomMethod()
    {
        return Results.Ok();
    }

    protected async ValueTask<IResult> SearchMethod(
        [FromServices] ShieldDbContext context,
        [FromServices] IMapper mapper,
        [AsParameters] CertificateSearch userSearch,
        HttpContext httpContext,
        CancellationToken token
    )
    {
        IQueryable<Certificate> query = await context
             .Set<Certificate>()
             .AsNoTracking()
             .Where(
                 x =>
                     x.CertificateData.Contains(userSearch.Keyword!)
                     || x.PrivateKey.Contains(userSearch.Keyword!)
             )
             .Sort<Certificate>("PrivateKey", SortDirectionEnum.Asc);


        IList<Certificate> resultList = await query
.Skip(userSearch.Page)
.Take(userSearch.PageSize)
.ToListAsync(token);

        return (resultList != null && resultList.Count > 0)
            ? Results.Ok(mapper.Map<IList<CertificateDTO>>(resultList))
            : Results.NoContent();
    }

}
