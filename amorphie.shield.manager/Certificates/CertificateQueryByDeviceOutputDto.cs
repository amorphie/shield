using amorphie.core.Base;
using amorphie.shield.Shared;

namespace amorphie.shield.Certificates;

public class CertificateQueryByDeviceOutputDto : CertificateQueryOutputDto
{
    public IdentityDto? Identity { get; set; }
}
