using System.Security.Cryptography.X509Certificates;
using amorphie.core.Module.minimal_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using amorphie.core.Identity;
using amorphie.core.Extension;
using amorphie.shield.Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.Extension;
using Microsoft.AspNetCore.Routing;

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
        group.MapGet("/status/user/{userTckn}/device/{xDeviceId}", GetByUserTcknAndXDeviceIdAsync);
        group.MapGet("/status/user/device/{xDeviceId}", GetByDeviceIdAsync);
        group.MapPost("/renew", CreateAsync);

    }

    static async ValueTask<IResult> CreateAsync([FromBody] CertificateCreateInputDto certificateCreateRequest, [FromServices] ICertificateAppService certificateService)
    {
        var dbResponse = await certificateService.CreateAsync(certificateCreateRequest);
        return ApiResult.CreateResult(dbResponse);
    }
    static async ValueTask<IResult> GetBySerialAsync(
    [FromRoute(Name = "certificateSerialNumber")] string certificateSerialNumber,
    [FromServices] ICertificateAppService certificateService
    )
    {
        var dbResponse = await certificateService.GetBySerialAsync(certificateSerialNumber);
        return ApiResult.CreateResult(dbResponse);
    }
    static async ValueTask<IResult> GetBySerialAndUserTcknAsync(
    [FromRoute(Name = "certificateSerialNumber")] string certificateSerialNumber,
    [FromRoute(Name = "userTckn")] string userTckn,
    [FromServices] ICertificateAppService certificateService
    )
    {
        var dbResponse = await certificateService.GetBySerialAndUserTcknAsync(certificateSerialNumber, userTckn);
        return ApiResult.CreateResult(dbResponse);
    }
    static async ValueTask<IResult> GetBySerialAndUserTcknAndXTokenIdAsync(
    [FromRoute(Name = "certificateSerialNumber")] string certificateSerialNumber,
    [FromRoute(Name = "userTckn")] string userTckn,
    [FromRoute(Name = "xTokenId")] Guid xTokenId,
    [FromServices] ICertificateAppService certificateService
    )
    {
        var dbResponse = await certificateService.GetBySerialAndUserTcknAndXTokenIdAsync(certificateSerialNumber, userTckn, xTokenId);
        return ApiResult.CreateResult(dbResponse);
    }
    static async ValueTask<IResult> GetByUserTcknAndXTokenIdAsync(
    [FromRoute(Name = "userTckn")] string userTckn,
    [FromRoute(Name = "xTokenId")] Guid xTokenId,
    [FromServices] ICertificateAppService certificateService
    )
    {
        var dbResponse = await certificateService.GetByUserTcknAndXTokenIdAsync(userTckn, xTokenId);
        return ApiResult.CreateResult(dbResponse);
    }
    static async ValueTask<IResult> GetByUserTcknAndXDeviceIdAsync(
    [FromRoute(Name = "userTckn")] string userTckn,
    [FromRoute(Name = "xDeviceId")] string xDeviceId,
    [FromServices] ICertificateAppService certificateService
    )
    {
        var dbResponse = await certificateService.GetByUserTcknAndXDeviceIdAsync(userTckn, xDeviceId);
        return ApiResult.CreateResult(dbResponse);
    }

    static async ValueTask<IResult> GetByDeviceIdAsync(
    [FromRoute(Name = "xDeviceId")] string xDeviceId,
    [FromServices] ICertificateAppService certificateService
    )
    {
        var dbResponse = await certificateService.GetByDeviceIdAsync(xDeviceId);
        return ApiResult.CreateResult(dbResponse);
    }

}
