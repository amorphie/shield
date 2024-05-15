namespace amorphie.shield.Shared;

public class IdentityDto
{
    public Guid DeviceId { get; set; }
    public Guid TokenId { get; set; }
    public Guid RequestId { get; set; }
    public string? UserTCKN { get; set; }
}
