using System.Security.Cryptography.X509Certificates;
using amorphie.core.Base;

namespace amorphie.shield.core.Dto.CaDto;

public class CaCreateDto : DtoBase
{
    public string? CaPem { get; set; }
    public string? RsaPem { get; set; }
    public X509Certificate2? Cert { get; set; }
}
