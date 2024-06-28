
namespace amorphie.shield.CertManager;
public class VaultOptions
{
    public const string Vault = "Vault";
    public const string VAULT_ADDR = "VAULT_ADDR";
    public const string VAULT_ROLE = "VAULT_ROLE";
    public string CommonName { get; set; } = default!;
    public int RSAKeySizeInBits { get; set; }
    public string RoleName { get; set; } = default!;
    public string TimeToLive { get; set; } = default!;
    public string PkiBackendMountPoint { get; set; } = default!;
}

