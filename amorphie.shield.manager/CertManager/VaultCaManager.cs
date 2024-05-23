using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace amorphie.shield.CertManager;
public class VaultCaManager : ICaManager
{
    private readonly string password = "password";

    private readonly IConfiguration _configuration;
    public VaultCaManager(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async ValueTask<X509Certificate2> GetFromPfxAsync()
    {
        var cerString = _configuration["CaCert"]!.ToString();
        var certBytes = Convert.FromBase64String(cerString);
        var certificate = new X509Certificate2(certBytes, password);
        var privateKeyString = _configuration["CaPrivateKey"]!.ToString();

        var privateKey = RSA.Create();
        privateKey.ImportFromPem(privateKeyString.ToCharArray());

        var caCert = certificate.CopyWithPrivateKey(privateKey);
        return caCert;
    }
}

