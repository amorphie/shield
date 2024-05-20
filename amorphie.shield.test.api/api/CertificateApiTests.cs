using amorphie.shield.Certificates;
using amorphie.shield.CertManager;
using System.Net.Http;

namespace amorphie.shield.api;
public class CertificateApiTests
{
    readonly HttpClient httpClient;
    public CertificateApiTests(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    [Fact]
    public async Task MinimalApiRouteResponseTest()
    {
        var responseText = await httpClient.GetStringAsync("/hello");
        Assert.Equal("Hello world", responseText);
    }
}

