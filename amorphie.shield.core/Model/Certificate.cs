using amorphie.core.Base;

namespace amorphie.shield.core.Model;

public class Certificate : EntityBase
{
    public Guid? XDeviceId { get; set; }
    public Guid? XTokenId { get; set; }
    public Guid? XRequestId { get; set; }
    public string? UserTCKN { get; set; }
    public string SerialNumber { get; set; } = default!;
    public string PublicCert { get; set; } = default!;
    public string? PrivateKey { get; set; }
    public string? TabPrint { get; set; }
    /// <summary>
    /// Active, Passive, Revoked 
    /// </summary>
    public string Status { get; set; } = default!;
    public DateTime RevocationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsCa { get; set; }

}
