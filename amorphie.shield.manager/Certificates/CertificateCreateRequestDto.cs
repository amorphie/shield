using amorphie.core.Base;
using amorphie.shield.Shared;

namespace amorphie.shield.Certificates;

public class CertificateCreateRequestDto : DtoBase
{
    public Guid? InstanceId { get; set; }
    public IdentityDto Identity { get; set; }
}
