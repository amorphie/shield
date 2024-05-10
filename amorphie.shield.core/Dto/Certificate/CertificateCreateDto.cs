using System.Security.Cryptography.X509Certificates;
using amorphie.core.Base;

namespace amorphie.shield.core.Dto.Certificate;

public class CertificateCreateDto : DtoBase
{
    public Guid? XDeviceId { get; set; }
    public Guid? XTokenId { get; set; }
    public Guid? XRequestId { get; set; }
    public string? PublicCert { get; set; }
    public string? PrivateKey { get; set; }
    public string? UserTCKN { get; set; }
    public string SerialNumber { get; set; } = default!;
    public X509Certificate2? Cert { get; set; }
}
