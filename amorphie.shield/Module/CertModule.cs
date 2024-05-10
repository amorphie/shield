using amorphie.shield.core.Model;
using amorphie.core.Module.minimal_api;
using amorphie.shield.data;
using Microsoft.AspNetCore.Mvc;
using amorphie.shield.core.Search;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using amorphie.core.Identity;
using amorphie.core.Extension;
using amorphie.shield.core.Dto.Certificate;
using amorphie.shield.app.CertManager;
using System.Security.Cryptography.X509Certificates;
using amorphie.shield.app.Db;
using amorphie.shield.core.Extension;

namespace amorphie.shield.Module;

public sealed class CertModule : BaseBBTRoute<CertificateDto, Certificate, ShieldDbContext>
{
    public CertModule(WebApplication app)
        : base(app) { }

    public override string[]? PropertyCheckList => new string[] { "FirstMidName", "LastName" };

    public override string? UrlFragment => "cert";

    public override void AddRoutes(RouteGroupBuilder routeGroupBuilder)
    {

        base.AddRoutes(routeGroupBuilder);

        routeGroupBuilder.MapGet("/search", SearchMethod);
        routeGroupBuilder.MapGet("/client-cert", GetClientCert);
        routeGroupBuilder.MapPost("/save", SaveAsync);
    }
    protected override ValueTask<IResult> UpsertMethod([FromServices] IMapper mapper, [FromServices] FluentValidation.IValidator<Certificate> validator, [FromServices] ShieldDbContext context, [FromServices] IBBTIdentity bbtIdentity, [FromBody] CertificateDto data, HttpContext httpContext, CancellationToken token)
    {
        return base.UpsertMethod(mapper, validator, context, bbtIdentity, data, httpContext, token);
    }
    protected async ValueTask<IResult> GetClientCert(
        [FromServices] ShieldDbContext context,
        [FromServices] CertificateManager certManager
        )
    {
        var ca = CaProvider.CaCert;
        var certificate = certManager.Create(ca, "", "testClient", "password");
        var cert = certificate.ExportCer();
        var privateKey = certificate.GetRSAPrivateKey().ExportPrivateKey();
        return Results.Ok((cert, privateKey));
    }
    protected async ValueTask<IResult> SaveAsync(
        [FromBody] CertificateCreateRequestDto certificateCreateRequest,
        [FromServices] CertificateService certificateService,
        [FromServices] CertificateManager certManager
        )
    {
        var ca = CaProvider.CaCert;
        var certificate = certManager.Create(ca, certificateCreateRequest.UserTCKN, "testClient", "password");
        var dbResponse = await certificateService.SaveAsync(certificateCreateRequest, certificate);
        return ApiResult.CreateResult(dbResponse);
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
                     x.PublicCert.Contains(userSearch.Keyword!)
                     || x.SerialNumber.Contains(userSearch.Keyword!)
             )
             .Sort<Certificate>("PrivateKey", SortDirectionEnum.Asc);


        IList<Certificate> resultList = await query
.Skip(userSearch.Page)
.Take(userSearch.PageSize)
.ToListAsync(token);

        return (resultList != null && resultList.Count > 0)
            ? Results.Ok(mapper.Map<IList<CertificateDto>>(resultList))
            : Results.NoContent();
    }

}
