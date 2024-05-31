using System.Net.Http.Json;
using amorphie.shield.Certificates;
using System.Text.Json;
using Helpers;
using amorphie.shield.test.Helpers;
using Xunit.Priority;
using amorphie.shield.test.usecase.Helpers;
using System.Net;


namespace amorphie.shield.test.usecase;
public class CertificateManagerWorkflowTest
{
    private readonly HttpClient _httpClient;
    static ManualResetEventSlim manualEvent = new ManualResetEventSlim(false); // Used to control the execution of the next
    public CertificateManagerWorkflowTest()
    {

        _httpClient = HttpClientHelper.GetHttpClient();
    }
    [Fact, Priority(0)]
    public async Task Cert_Managemenet_Start()
    {
        var transitionName = "cert-management-start";
        var body = new CertificateCreateInputDto
        {
            InstanceId = StaticData.InstanceId,
            Identity = new Shared.IdentityDto
            {
                DeviceId = StaticData.XDeviceId.ToString(),
                TokenId = StaticData.XTokenId,
                RequestId = StaticData.XRequestId,
                UserTCKN = StaticData.UserTCKN
            }
        };

        var hub = new HubClientHelper();
        hub.MessageReceived += (sender, e) =>
        {
            var signalRdata = JsonSerializer.Deserialize<FakeSignalRData>(e);
            Assert.NotNull(signalRdata);
            if (signalRdata.subject == "worker-completed")
            {
                var innerData = signalRdata.data!.GetProperty("additionalData").GetProperty("certCreateResult").GetProperty("data");
                string certificate = innerData.GetProperty("certificate").ToString();

                Assert.StartsWith("-----BEGIN CERTIFICATE-----", certificate);

                manualEvent.Set();

            }

        };
        await hub.ConnectAsync();

        var response = await _httpClient.PostAsJsonAsync($"workflow/instance/{StaticData.InstanceId}/transition/{transitionName}", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.IsSuccessStatusCode);
        manualEvent.Wait();
    }
}
