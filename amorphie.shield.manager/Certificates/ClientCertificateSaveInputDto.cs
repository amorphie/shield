using amorphie.core.Base;
using amorphie.shield.Shared;

namespace amorphie.shield.Certificates;

public class ClientCertificateSaveInputDto : DtoBase
{
    public Guid? InstanceId { get; set; }
    public IdentityDto Identity { get; set; }
    public required string PublicKey { get; set; }
    public required string SerialNumber { get; set; }
    public required string CommonName { get; set; }
    public required string Thumbprint { get; set; }
    public DateTime ExpirationDate { get; set; }

}
