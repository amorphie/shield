using amorphie.core.Base;

namespace amorphie.shield.core.Model;

public class Certificate : EntityBase
{
    /// <summary>
    /// Workflow instanceid
    /// </summary>
    public Guid? InstanceId { get; set; }
    public Guid? XDeviceId { get; set; } //XDevice id ile bir tane aktif cert olacak
    public Guid? XTokenId { get; set; }
    public Guid? XRequestId { get; set; }
    public string? UserTCKN { get; set; }
    public string? Cn { get; set; }
    public string SerialNumber { get; set; } = default!;
    public string PublicCert { get; set; } = default!;
    public string? ThumbPrint { get; set; }
    /// <summary>
    /// Active, Passive, Revoked, Expired
    /// </summary>
    public string Status { get; set; } = default!;
    public string? StatusReason { get; set; }
    public DateTime? RevocationDate { get; set; }
    public DateTime ExpirationDate { get; set; }

}
