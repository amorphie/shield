using System.Security.Cryptography.X509Certificates;
using amorphie.core.Base;

namespace amorphie.shield.Certificates;

public class CertificateCreateDto : DtoBase
{
    public Guid? DeviceId { get; set; }
    public Guid? TokenId { get; set; }
    public Guid? RequestId { get; set; }
    public string? PublicCert { get; set; }
    public string? PrivateKey { get; set; }
    public string? UserTCKN { get; set; }
    public string SerialNumber { get; set; } = default!;
    public X509Certificate2? Cert { get; set; }
}
