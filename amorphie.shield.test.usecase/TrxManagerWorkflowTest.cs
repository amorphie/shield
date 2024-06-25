using System.Net.Http.Json;
using amorphie.shield.Certificates;
using amorphie.core.Base;
using amorphie.core.Enums;
using System.Text.Json;
using Helpers;
using amorphie.shield.test.Helpers;
using Xunit.Priority;
using amorphie.shield.test.usecase.Helpers;
using amorphie.shield.Transactions;
using System.Text.Json.Nodes;


namespace amorphie.shield.test.usecase;
public class TrxManagerWorkflowTest
{
    private readonly HttpClient _httpClient;
    static ManualResetEventSlim manualEvent = new ManualResetEventSlim(false); // Used to control the execution of the next
    public TrxManagerWorkflowTest()
    {
        _httpClient = HttpClientHelper.GetHttpClient();

    }
    [Fact, Priority(0)]
    public async Task Trx_Managemenet_Start()
    {
        var transitionName = "transaction-management-start";
        var body = new CreateTransactionInput
        {
            InstanceId = StaticData.InstanceId,
            Identity = new Shared.IdentityDto
            {
                DeviceId = StaticData.XDeviceId.ToString(),
                TokenId = StaticData.XTokenId,
                RequestId = StaticData.XRequestId,
                UserTCKN = "2329"
            },
            
        };
        body.Data = new Dictionary<string, object>
        {
            { "TobeSignData", "TestData" }
        };

        var hub = new HubClientHelper();
        hub.MessageReceived += (sender, e) =>
        {
            var signalRdata = JsonSerializer.Deserialize<JsonObject>(e);
            if (signalRdata["subject"].ToString() == "worker-completed")
            {
                var createTransactionOutput = signalRdata["data"]?["additionalData"]?["encryptResult"]?["data"];
                var encryptData = createTransactionOutput["encryptData"].ToString();
                //decrypt


                manualEvent.Set();

            }

        };
        await hub.ConnectAsync();

        var response = await _httpClient.PostAsJsonAsync($"workflow/instance/{StaticData.InstanceId}/transition/{transitionName}", body);

        var status = response.StatusCode;
        if (response.IsSuccessStatusCode)
        {

        }
        manualEvent.Wait();


    }

    [Fact, Priority(1)]
    public async Task Trx_Managemenet_Sign()
    {
        var transitionName = "transaction-management-send-sign";
        var body = new CertificateCreateInputDto
        {
            InstanceId = StaticData.InstanceId,
            Identity = new Shared.IdentityDto
            {
                DeviceId = StaticData.XDeviceId.ToString(),
                TokenId = StaticData.XTokenId,
                RequestId = StaticData.XRequestId,
                UserTCKN = "2329"
            }
        };

        var hub = new HubClientHelper();
        hub.MessageReceived += (sender, e) =>
        {
            var signalRdata = JsonSerializer.Deserialize<FakeSignalRData>(e);
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

        var status = response.StatusCode;
        if (response.IsSuccessStatusCode)
        {

        }
        manualEvent.Wait();


    }

}
