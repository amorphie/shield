using Microsoft.AspNetCore.Mvc;
using amorphie.shield.Certificates;

namespace amorphie.shield.Module;

public static class ClientCertificateModule
{

    public static void RegisterClientCertificateModuleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("client-certificate");
        group.MapPost("/save", SaveAsync);
    }

    internal static async ValueTask<IResult> SaveAsync([FromBody] ClientCertificateSaveInputDto clientCertificateSaveInputDto, [FromServices] ICertificateAppService certificateService)
    {
        var dbResponse = await certificateService.SaveClientCertAsync(clientCertificateSaveInputDto);
        return ApiResult.CreateResult(dbResponse);
    }
  

}
