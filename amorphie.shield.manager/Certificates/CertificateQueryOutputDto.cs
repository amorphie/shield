using amorphie.core.Base;
using amorphie.shield.Shared;

namespace amorphie.shield.Certificates;

public class CertificateQueryOutputDto
{
    public bool Valid { get; set; }
    public DateTime ExpirationDate { get; set; }
    public IdentityDto? Identity { get; set; }


}
