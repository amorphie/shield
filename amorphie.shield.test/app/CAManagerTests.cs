using amorphie.shield.CertManager;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using amorphie.shield.Extension;
using amorphie.shield.test.Helpers;

namespace amorphie.shield.test;
public class CaManagerTests
{
    [Fact]
    public void Create_ReturnsCaCreateDto_WhenCaCreated()
    {
        var cn = "dev.ca.burganbank";

        var name = "ca";
        var caManager = new CaManager();
        var result = caManager.Create(cn, StaticData.Password);
        var caCerPem = result.ExportCertificatePem();
        var caPfx = result.Export(X509ContentType.Pkcs12, StaticData.Password);
        File.WriteAllBytes($"{StaticData.CaCertBasePath}{name}.pfx", caPfx);
        File.WriteAllText($"{StaticData.CaCertBasePath}{name}.cer", caCerPem);

        var privateKey = result.GetRSAPrivateKey()?.ExportPrivateKey();
        File.WriteAllText($"{StaticData.CaCertBasePath}{name}.private.key", privateKey);

        var publicKey = result.GetRSAPrivateKey()?.ExportRSAPublicKeyPem();
        File.WriteAllText($"{StaticData.CaCertBasePath}{name}.public.key", publicKey);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<X509Certificate2>(result);
    }
    [Fact]
    public void Generate_Pfx_From_Cer_And_PrivateKey()
    {
        // Read the certificate
        var certBytes = CertificateHelper.GetCaCertFromFile();
        var certificate = new X509Certificate2(certBytes, StaticData.Password);

        // Read the private key
        var privateKey = CertificateHelper.GetCaPrivateKeyFromFile_RSA();

        // Combine into an X509Certificate2 object with the private key
        var certWithKey = certificate.CopyWithPrivateKey(privateKey);

        // Export to PFX
        byte[] pfxBytes = certWithKey.Export(X509ContentType.Pfx, StaticData.Password);
        File.WriteAllBytes(Path.Combine(StaticData.CaCertBasePath, "ca.pfx"), pfxBytes);

        // Assert
        Assert.NotNull(pfxBytes);
    }
    [Fact]
    public void Inspect_Certificate()
    {

        string certPath = "newCer.cer"; // Path to your certificate file
        //string keyPath = ""; // Path to your certificate file
        string keyPath = "newPrivateKey.private.key";  // Path to your private key file



        // Read the certificate
        var certBytes = File.ReadAllBytes(certPath);
        var certificate = new X509Certificate2(certBytes, StaticData.Password);

        // Read the private key
        var privateKeyText = File.ReadAllText(keyPath);
        var privateKey = RSA.Create();
        privateKey.ImportFromPem(privateKeyText.ToCharArray());

        // Combine into an X509Certificate2 object with the private key
        var certWithKey = certificate.CopyWithPrivateKey(privateKey);
        //var friendlyName = certWithKey.FriendlyName;
        var thumbPrint = certWithKey.Thumbprint;
        var serialNumber = certWithKey.SerialNumber;
        var issuer = certWithKey.Issuer;


        // Assert
        Assert.IsType<X509Certificate2>(certificate);
        Assert.NotNull(thumbPrint);
        Assert.NotNull(serialNumber);
        Assert.NotNull(issuer);
    }
}

