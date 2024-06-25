
namespace amorphie.shield.CertManager;
public class VaultOptions
{
    public const string Vault = "Vault";
    public required string CommonName { get; set; }
    public int RSAKeySizeInBits { get; set; }
    public required string RoleName { get; set; }
    public required string TimeToLive { get; set; }
}

