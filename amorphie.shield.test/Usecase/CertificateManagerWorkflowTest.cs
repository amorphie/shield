using System.Net.Http.Json;
using amorphie.shield.Certificates;
using amorphie.core.Base;
using amorphie.core.Enums;
using System.Text.Json;
using Helpers;
using amorphie.shield.test.Helpers;
using Xunit.Priority;
using amorphie.shield.test.usecase.Helpers;


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
