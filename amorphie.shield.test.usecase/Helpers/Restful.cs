using System.Text;
using System.Text.Json;

namespace amorphie.shield.test.Helpers;

public class Restful
{
    public async Task<TResponse> CallServicePost<TRequest, TResponse>(string Url, object json, Dictionary<string, string> acceptHeaders, bool? authorizationHeader)
    {

        var serializeReq = JsonSerializer.Serialize(json);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, Url);
        httpRequestMessage.Content = new StringContent(serializeReq, Encoding.UTF8, "application/json");

        if (acceptHeaders != null && acceptHeaders.Any())
        {
            foreach (var item in acceptHeaders)
            {
                httpRequestMessage.Headers.Add(item.Key, item.Value);
            }
        }

        try
        {
            HttpClient client = new HttpClient();
            if (authorizationHeader.GetValueOrDefault(false))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + StaticData.Token);
            }
            //var content = new StringContent(serializeReq, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
            var responseCode = response.StatusCode.ToString();
            var deserializeRes = JsonSerializer.Deserialize<TResponse>(await response.Content.ReadAsStringAsync());
            return deserializeRes;
        }
        catch (Exception e)
        {
            Console.WriteLine("CallServicePost Hata :" + e.Message);
            throw;
        }
    }

    public async Task<TResponse> CallServiceGet<TResponse>(string Url, object json, Dictionary<string, string> acceptHeaders, bool authorizationHeader)
    {
        var serializeReq = JsonSerializer.Serialize(json);

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, Url);
        httpRequestMessage.Content = new StringContent(serializeReq, Encoding.UTF8, "application/json");


        try
        {
            HttpClient client = new HttpClient();
            if (authorizationHeader)
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + StaticData.Token);
            }
            HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
            var responseCode = response.StatusCode.ToString();
            var deserializeRes = JsonSerializer.Deserialize<TResponse>(await response.Content.ReadAsStringAsync());
            return deserializeRes;
        }
        catch (Exception e)
        {
            Console.WriteLine("CallServiceGet Hata :" + e.Message);
            throw;
        }
    }
    public async Task<string> CallServicePut(string Url, object jsonObject)
    {
        try
        {
            HttpClient client = new HttpClient();
            var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
            content.Headers.Add("user", "EBT\\INTERNETUSER");
            content.Headers.Add("channel", "INTERNET");
            content.Headers.Add("branch", "9530");
            HttpResponseMessage response = await client.PutAsync(Url, content);
            var responseCode = response.StatusCode.ToString();
            var responseMessage = response.ReasonPhrase;
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch (Exception e)
        {
            Console.WriteLine("CallServicePut Hata :" + e.Message);
            throw;
        }
    }
}

