using amorphie.core.Base;

public class FakeSignalRData : EntityBase
{
    public string? InstanceId { get; set; }
    public string? source { get; set; }
    public string? type { get; set; }
    public string? subject { get; set; }
    public dynamic? data { get; set; }

    public string? mfatype { get; set; }
    public DateTime? time { get; set; }
    public string? datacontenttype { get; set; }
    public string? specversion { get; set; }
    public string? deviceId { get; set; }
    public string? userId { get; set; }
    public string? tokenId { get; set; }
    public string? requestId { get; set; }
    public bool? routeChange { get; set; }
    public string? pageUrl { get; set; }
    public string? navigationType { get; set; }
}
