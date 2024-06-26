using Microsoft.AspNetCore.Mvc;
using amorphie.shield.Certificates;
using amorphie.shield.Extension;

namespace amorphie.shield.Module;

public static class CertificateModule
{

    public static void RegisterCertificateModuleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("certificate");
        group.MapPost("/create", CreateAsync);
        group.MapGet("/status/serial/{certificateSerialNumber}", GetBySerialAsync);
        group.MapGet("/status/serial/{certificateSerialNumber}/user/{userTckn}", GetBySerialAndUserTcknAsync);
        group.MapGet("/status/serial/{certificateSerialNumber}/user/{userTckn}/token/{xTokenId}", GetBySerialAndUserTcknAndXTokenIdAsync);
        group.MapGet("/status/user/{userTckn}/token/{xTokenId}", GetByUserTcknAndXTokenIdAsync);
        group.MapGet("/status/user/{userTckn}/device/{xDeviceId}/{origin}", GetByUserTcknAndXDeviceIdAsync);
        group.MapGet("/status/user/device/{xDeviceId}", GetByDeviceIdAsync);
        group.MapPost("/renew", CreateAsync);

    }

    internal static async ValueTask<IResult> CreateAsync([FromBody] CertificateCreateInputDto certificateCreateRequest, [FromServices] ICertificateAppService certificateService)
    {
        var dbResponse = await certificateService.CreateAsync(certificateCreateRequest);
        return ApiResult.CreateResult(dbResponse);
    }
    internal static async ValueTask<IResult> GetBySerialAsync(
    [FromRoute(Name = "certificateSerialNumber")] string certificateSerialNumber,
    [FromServices] ICertificateAppService certificateService
    )
    {
        var dbResponse = await certificateService.GetBySerialAsync(certificateSerialNumber);
        return ApiResult.CreateResult(dbResponse);
    }
    internal static async ValueTask<IResult> GetBySerialAndUserTcknAsync(
    [FromRoute(Name = "certificateSerialNumber")] string certificateSerialNumber,
    [FromRoute(Name = "userTckn")] string userTckn,
    [FromServices] ICertificateAppService certificateService
    )
    {
        var dbResponse = await certificateService.GetBySerialAndUserTcknAsync(certificateSerialNumber, userTckn);
        return ApiResult.CreateResult(dbResponse);
    }
    internal static async ValueTask<IResult> GetBySerialAndUserTcknAndXTokenIdAsync(
    [FromRoute(Name = "certificateSerialNumber")] string certificateSerialNumber,
    [FromRoute(Name = "userTckn")] string userTckn,
    [FromRoute(Name = "xTokenId")] Guid xTokenId,
    [FromServices] ICertificateAppService certificateService
    )
    {
        var dbResponse = await certificateService.GetBySerialAndUserTcknAndXTokenIdAsync(certificateSerialNumber, userTckn, xTokenId);
        return ApiResult.CreateResult(dbResponse);
    }
    internal static async ValueTask<IResult> GetByUserTcknAndXTokenIdAsync(
    [FromRoute(Name = "userTckn")] string userTckn,
    [FromRoute(Name = "xTokenId")] Guid xTokenId,
    [FromServices] ICertificateAppService certificateService
    )
    {
        var dbResponse = await certificateService.GetByUserTcknAndXTokenIdAsync(userTckn, xTokenId);
        return ApiResult.CreateResult(dbResponse);
    }
    internal static async ValueTask<IResult> GetByUserTcknAndXDeviceIdAsync(
    [FromRoute(Name = "userTckn")] string userTckn,
    [FromRoute(Name = "xDeviceId")] string xDeviceId,
    [FromRoute(Name = "origin")] string origin,
    [FromServices] ICertificateAppService certificateService
    )
    {
        CertificateOrigin certificateOrigin = (CertificateOrigin)Enum.Parse(typeof(CertificateOrigin), origin.FirstCharToUpper());
        var dbResponse = await certificateService.GetByUserTcknAndXDeviceIdAsync(userTckn, xDeviceId, certificateOrigin);
        return ApiResult.CreateResult(dbResponse);
    }

    internal static async ValueTask<IResult> GetByDeviceIdAsync(
    [FromRoute(Name = "xDeviceId")] string xDeviceId,
    [FromServices] ICertificateAppService certificateService
    )
    {
        var dbResponse = await certificateService.GetByDeviceIdAsync(xDeviceId);
        return ApiResult.CreateResult(dbResponse);
    }

}
