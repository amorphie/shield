using amorphie.core.Base;

namespace amorphie.shield.Certificates;

public class CertificateCreateResponseDto : DtoBase
{
    public string? Certificate { get; set; }
    public string? PrivateKey { get; set; }
    public DateTime ExpirationDate { get; set; }

}
