using amorphie.core.Base;

namespace amorphie.shield.Certificates;

public class CertificateQueryOutputDto
{
    public bool Valid { get; set; }
    public DateTime ExpirationDate { get; set; }

}
