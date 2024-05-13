using Microsoft.AspNetCore.Mvc;
using amorphie.shield.Revokes;

namespace amorphie.shield.Module;

public static class RevokeModule
{

    public static void RegisterRevokeModuleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("certificate/revoke");

        group.MapGet("/token/{xTokenId}", RevokeTokenAsync);
        group.MapGet("/device/{xDeviceId}", RevokeDeviceAsync);
        group.MapGet("/user/{userTckn}", RevokeUserAsync);
        group.MapGet("/id/{certificateId}", RevokeCertificateAsync);
    }

    static async ValueTask<IResult> RevokeTokenAsync(
    [FromRoute(Name = "xTokenId")] Guid xTokenId,
    [FromServices] IRevokeAppService revokeService
    )
    {
        var dbResponse = await revokeService.RevokeTokenAsync(xTokenId);
        return ApiResult.CreateResult(dbResponse);
    }
    static async ValueTask<IResult> RevokeDeviceAsync(
    [FromRoute(Name = "xDeviceId")] Guid xDeviceId,
    [FromServices] IRevokeAppService revokeService
    )
    {
        var dbResponse = await revokeService.RevokeDeviceAsync(xDeviceId);
        return ApiResult.CreateResult(dbResponse);
    }
    static async ValueTask<IResult> RevokeUserAsync(
    [FromRoute(Name = "userTckn")] string userTckn,
    [FromServices] IRevokeAppService revokeService
    )
    {
        var dbResponse = await revokeService.RevokeUserAsync(userTckn);
        return ApiResult.CreateResult(dbResponse);
    }
    static async ValueTask<IResult> RevokeCertificateAsync(
    [FromRoute(Name = "certificateId")] Guid certificateId,
    [FromServices] IRevokeAppService revokeService
    )
    {
        var dbResponse = await revokeService.RevokeCertificateAsync(certificateId);
        return ApiResult.CreateResult(dbResponse);
    }

}
