using amorphie.shield.test.integration;
using System.Net.Http.Json;
using amorphie.shield.Certificates;

namespace amorphie.shield.integration;
public class CertificateManagerTest : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;

    public CertificateManagerTest(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }
    [Fact]
    public async Task Get_Certificate_By_DeviceId()
    {
        var deviceId = "a0000f64-5717-0000-b3fc-2d963f660004";
        var response = await _httpClient.GetAsync("/certificate/status/user/device/" + deviceId);
    }
    [Fact]
    public async Task Create_New_Certificate()
    {
        var deviceId = "a0000f64-5717-0000-b3fc-2d963f660004";
        var createInput = new CertificateCreateInputDto
        {
            Id = Guid.NewGuid(),
            Identity = new Shared.IdentityDto
            {
                DeviceId = deviceId,
                RequestId = Guid.NewGuid()
            }
        };
            var response = await _httpClient.PostAsJsonAsync("/certificate/create", createInput);
    }
    [Fact]
    public async Task Revoke_Existing_And_Create_New_Certificate()
    {
        var deviceId = "a0000f64-5717-0000-b3fc-2d963f660004";
        var response = await _httpClient.GetAsync("/certificate/status/user/device/" + deviceId);
    }
}
