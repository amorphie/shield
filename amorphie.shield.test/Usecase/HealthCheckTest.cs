using amorphie.shield.test.usecase.Helpers;
using System.Net;


namespace amorphie.shield.test.usecase;
public class HealthCheckTest
{
    private readonly HttpClient _httpClient;
    public HealthCheckTest()
    {

        _httpClient = HttpClientHelper.GetHttpClient();
    }
    [Fact]
    public async Task HealthCheck()
    {
        try
        {
            var response = await _httpClient.GetAsync($"health");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Assert.NotNull(content);
            }

        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
            throw;
        }
    }
}
