using amorphie.shield.test.Helpers;

namespace amorphie.shield.test.usecase.Helpers;
public static class HttpClientHelper
{
    public static HttpClient GetHttpClient()
    {
        var _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:4201/"),
        };
        _httpClient.DefaultRequestHeaders.Add("X-Device-Id", StaticData.XDeviceId.ToString());
        _httpClient.DefaultRequestHeaders.Add("X-Token-Id", StaticData.XTokenId.ToString());
        _httpClient.DefaultRequestHeaders.Add("X-Request-Id", StaticData.XRequestId.ToString());

        _httpClient.DefaultRequestHeaders.Add("User", "3fa85f64-5717-4562-b3fc-2c963f66afa6");
        _httpClient.DefaultRequestHeaders.Add("Behalf-Of-User", "3fa85f64-5717-4562-b3fc-2c963f66afa6");
        return _httpClient;
    }
}

