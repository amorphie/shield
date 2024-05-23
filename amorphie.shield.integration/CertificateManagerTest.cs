using amorphie.shield.test.integration;
using System.Net.Http.Json;
using amorphie.shield.Certificates;
using amorphie.core.Base;
using amorphie.core.Enums;
using amorphie.shield.Revokes;
using System.Text.Json;
using amorphie.shield.test.integration.Helpers;
using Microsoft.AspNetCore.Http;


namespace amorphie.shield.integration;
public class CertificateManagerTest : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;
    private readonly string existingDeviceId = "a0000f64-5717-0000-b3fc-2d963f660004";
    public CertificateManagerTest(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
        CreateNewCertificate(existingDeviceId).Wait();
    }
    [Fact]
    public async Task Get_Certificate_By_DeviceId()
    {
        var deviceId = "a0000f64-5717-0000-b3fc-2d963f660000";
        var response = await _httpClient.GetAsync("/certificate/status/user/device/" + deviceId);
    }
    [Fact]
    public async Task Create_New_Certificate()
    {
        var deviceId = "a0000f64-5717-0000-b3fc-2d963f660002";
        var creationResult = await CreateNewCertificate(deviceId);

        Assert.NotNull(creationResult);
        Assert.Equal(Status.Success.ToString(), creationResult.Result.Status);
    }
    [Fact]
    public async Task Revoke_Existing_And_Create_New_Certificate()
    {
        //Get
        var response = await _httpClient.GetAsync("/certificate/status/user/device/" + existingDeviceId);
        var fakeDeviceGetResult = await response.Content.ReadFromJsonAsync<FakeResponse<CertificateQueryByDeviceOutputDto>>();
        Assert.NotNull(fakeDeviceGetResult);

        var deviceGetResult = TypeConvert(fakeDeviceGetResult);
        
        Assert.Equal(Status.Success.ToString(), deviceGetResult.Result.Status);
        Assert.True(deviceGetResult.Data.Valid);

        //Revoke
        var revokeResponse = await _httpClient.GetAsync("/certificate/revoke/device/" + existingDeviceId);
        var fakeRevokeResult = await revokeResponse.Content.ReadFromJsonAsync<FakeResponse<RevokeDeviceOutput>>();
        Assert.NotNull(fakeRevokeResult);

        var revokeResult = TypeConvert(fakeRevokeResult);
        Assert.Equal(Status.Success.ToString(), revokeResult.Result.Status);
        Assert.True(revokeResult.Data.Revoked);

        //Create
        var creationResult = await CreateNewCertificate(existingDeviceId);

        Assert.NotNull(creationResult);
        Assert.Equal(Status.Success.ToString(), creationResult.Result.Status);
    }


    private async Task<Response<CertificateCreateOutputDto>?> CreateNewCertificate(string deviceId)
    {
        var createInput = new CertificateCreateInputDto
        {
            Id = Guid.NewGuid(),
            InstanceId = Guid.NewGuid(),
            Identity = new Shared.IdentityDto
            {
                DeviceId = deviceId,
                RequestId = Guid.NewGuid(),
                UserTCKN = "34000491799",
                TokenId = Guid.NewGuid()
            }
        };
        var response = await _httpClient.PostAsJsonAsync("/certificate/create", createInput);
        var httpResponse = await response.Content.ReadFromJsonAsync<FakeResponse<CertificateCreateOutputDto>>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (httpResponse == null)
        {
            return null;
        }
        
        var result = TypeConvert(httpResponse);
        return result;

    }
    /// <summary>
    /// Result type in amorphie.core is in order to be deserialized, it must have parametreless ctro or current ctor must have JsonConstructorAttribute attribute
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fakeResponse"></param>
    /// <returns></returns>
    //Typeconvert
    private Response<T> TypeConvert<T>(FakeResponse<T> fakeResponse)
    {
        return new Response<T>
        {
            Data = fakeResponse.Data,
            Result = new Result(Status.Success, fakeResponse.Result.Message)
            {
                Message = fakeResponse.Result.Message,
                MessageDetail = fakeResponse.Result.MessageDetail,
                Status = fakeResponse.Result.Status
            }
        };
    }
}
