using System.Net.Http.Json;
using System.Text.Json;
using Helpers;
using amorphie.shield.test.Helpers;
using Xunit.Priority;
using amorphie.shield.test.usecase.Helpers;
using amorphie.shield.Transactions;
using System.Text.Json.Nodes;
using amorphie.shield.CertManager;


namespace amorphie.shield.test.usecase;
public class TrxManagerWorkflowTest
{
    private readonly HttpClient _httpClient;
    static ManualResetEventSlim manualEvent = new ManualResetEventSlim(false); // Used to control the execution of the next
    private string? _encryptData;
    private string? _signData;
    private Dictionary<string, object>? _data = new Dictionary<string, object>();
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
        _data.Add("TobeSignData", "TestData");
        body.Data = _data;

        var hub = new HubClientHelper();
        hub.MessageReceived += (sender, e) =>
        {
            var signalRdata = JsonSerializer.Deserialize<JsonObject>(e);
            if (signalRdata["subject"].ToString() == "worker-completed")
            {
                var createTransactionOutput = signalRdata["data"]?["additionalData"]?["encryptResult"]?["data"];
                Assert.NotNull(createTransactionOutput);

                _encryptData = createTransactionOutput["encryptData"]?.ToString();
                Assert.NotNull(_encryptData);
                var rawData = createTransactionOutput["rawData"];
                Assert.NotNull(rawData);
                _data = JsonSerializer.Deserialize<Dictionary<string, object>>(rawData);

                var encBytes = Convert.FromBase64String(_encryptData);
                //decrypt is client process
                //in decrypt process private and public key must be same certificate siblings
                //
                var cerPrivateKey = CertificateHelper.GetClientPrivateKeyFromFile();
                var decryptResult = CertificateManager.DecryptDataWithPrivateKey(encBytes, cerPrivateKey);

                _signData = CertificateManager.SignDataWithRSA(decryptResult, cerPrivateKey);

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
    public async Task Trx_Managemenet_Send_Signed()
    {
        var transitionName = "transaction-management-send-sign";
        var body = new VerifyTransactionInput
        {
            Identity = new Shared.IdentityDto
            {
                DeviceId = StaticData.XDeviceId.ToString(),
                TokenId = StaticData.XTokenId,
                RequestId = StaticData.XRequestId,
                UserTCKN = "2329"
            },
            RawData = _data!,
            SignData = _signData!

        };

        var hub = new HubClientHelper();
        hub.MessageReceived += (sender, e) =>
        {
            var signalRdata = JsonSerializer.Deserialize<JsonObject>(e);
            if (signalRdata["subject"].ToString() == "worker-completed")
            {
                //check verify result

                var verifyTransactionOutput = signalRdata["data"]?["additionalData"]?["verifyResult"]?["data"];
                Assert.NotNull(verifyTransactionOutput);

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
