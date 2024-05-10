using System.Security.Cryptography.X509Certificates;
using amorphie.core.Base;

namespace amorphie.shield.core.Dto.Certificate;

public class CertificateCreateRequestDto : DtoBase
{
    public Guid? InstanceId { get; set; }
    public Guid? XDeviceId { get; set; }
    public Guid? XTokenId { get; set; }
    public Guid? XRequestId { get; set; }
    public string UserTCKN { get; set; } = string.Empty;
}
