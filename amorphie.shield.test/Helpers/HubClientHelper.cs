using amorphie.shield.test.Helpers;
using Microsoft.AspNetCore.SignalR.Client;

namespace Helpers;
public delegate object MethodToRun();
public partial class HubClientHelper
{
    public HubClientHelper()
    {
        
    }

    public async Task ConnectAsync()
    {
       var connection = new HubConnectionBuilder()
            .WithUrl($"http://localhost:4203/hubs/genericHub?X-Device-Id={StaticData.XDeviceId}&X-Token-Id={StaticData.XTokenId}")
            .Build();

        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await ConnectAsync();
        };
        try
        {
            await connection.StartAsync();
        }
        catch (Exception ex)
        {
            throw;
        }

        connection.On<string>("SendMessage", (message) =>
        {     
            OnMessageReceived(message);
        });

    }

    protected virtual void OnMessageReceived(string e)
    {
        MessageReceived?.Invoke(this, e);
    }

    public event EventHandler<string>? MessageReceived;
}
