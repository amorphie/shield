using amorphie.shield.test.Helpers;
using Microsoft.AspNetCore.SignalR.Client;

namespace Helpers;
public delegate object MethodToRun();
public partial class HubClientHelper
{
    //readonly HubConnection connection;
    static List<string> messagesList = new();
    public HubClientHelper()
    {
        
    }

    public async Task ConnectAsync()
    {
       var connection = new HubConnectionBuilder()
            .WithUrl($"http://localhost:4203/hubs/genericHub?X-Device-Id={StaticData.XDeviceId}&X-Token-Id={StaticData.XTokenId}&X-Request-Id={StaticData.XRequestId}")

            .Build();

        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await ConnectAsync();
        };
        try
        {
            await connection.StartAsync();
            messagesList.Add("Connection started");

        }
        catch (Exception ex)
        {
            messagesList.Add(ex.Message);
        }

        connection.On<string>("SendMessage", (message) =>
        {
            var newMessage = $"{message}";
            messagesList.Add(newMessage);
        
            OnMessageReceived(message);
        });

    }

    protected virtual void OnMessageReceived(string e)
    {
        EventHandler<string> handler = MessageReceived;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public event EventHandler<string> MessageReceived;
}
