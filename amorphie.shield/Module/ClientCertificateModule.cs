using Microsoft.AspNetCore.Mvc;
using amorphie.shield.Certificates;

namespace amorphie.shield.Module;

public static class ClientCertificateModule
{

    public static void RegisterClientCertificateModuleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("client-certificate")
        ;
        group.MapPost("/save", SaveAsync)
        .Produces<ClientCertificateSaveInputDto>(StatusCodes.Status201Created, "application/json", "text/plain")
        .Accepts<ClientCertificateSaveInputDto>("application/json","text/plain")       
        ;
        group.MapPost("/encode", Encode)
        .Produces<ClientCertificateSaveInputDto>(StatusCodes.Status201Created, "application/json", "text/plain")
        .Accepts<ClientCertificateSaveInputDto>("application/json","text/plain")        
        ;
    }

    internal static async ValueTask<IResult> SaveAsync([FromBody] ClientCertificateSaveInputDto clientCertificateSaveInputDto, [FromServices] ICertificateAppService certificateService)
    {
        byte[] data = Convert.FromBase64String(clientCertificateSaveInputDto.PublicKey);
        clientCertificateSaveInputDto.PublicKey= System.Text.Encoding.UTF8.GetString(data);
        var dbResponse = await certificateService.SaveClientCertAsync(clientCertificateSaveInputDto);
        return ApiResult.CreateResult(dbResponse);
    }


    internal static async ValueTask<string> Encode(HttpContext c)
    {
        using var reader = new StreamReader(c.Request.Body);
        var publicKey = await reader.ReadToEndAsync();
        var publicKeyBytes = System.Text.Encoding.UTF8.GetBytes(publicKey);
        return Convert.ToBase64String(publicKeyBytes);
    }


}
