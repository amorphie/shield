using amorphie.shield.CertManager;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using amorphie.shield.Extension;

namespace amorphie.shield.app;
public class CaManagerTests
{
    [Fact]
    public void Create_ReturnsCaCreateDto_WhenCaCreated()
    {
        var cn = "dev.ca.burganbank";
        var password = "password";

        var caManager = new CaManager();
        var result = caManager.Create(cn, password);
        var cer =result.ExportCer();
        var privateKey = result.GetRSAPrivateKey()?.ExportPrivateKey();
        

        result.ExportCer("newCer");
        result.GetRSAPrivateKey()?.ExportPrivateKey("newPrivateKey");

        // Assert
        Assert.NotNull(result);
        Assert.IsType<X509Certificate2>(result);
    }
    [Fact]
    public void Generate_Pfx_From_Cer_And_PrivateKey()
    {
        //string certPath = Path.Combine("Certficate", "ca.cer"); // Path to your certificate file
        string certPath = "newCer.cer"; // Path to your certificate file
        //string keyPath = ""; // Path to your certificate file
        string keyPath = "newPrivateKey.private.key";  // Path to your private key file
        string pfxPath = Path.Combine("Certficate", "ca.pfx");          // Path where the PFX file will be saved
        string pfxPassword = "password";        // Password for the PFX file

        // Read the certificate
        var certBytes = File.ReadAllBytes(certPath);
        var certificate = new X509Certificate2(certBytes, pfxPassword);

        // Read the private key
        var privateKeyText = File.ReadAllText(keyPath);
        var privateKey = RSA.Create();
        privateKey.ImportFromPem(privateKeyText.ToCharArray());

        // Combine into an X509Certificate2 object with the private key
        var certWithKey = certificate.CopyWithPrivateKey(privateKey);

        // Export to PFX
        byte[] pfxBytes = certWithKey.Export(X509ContentType.Pfx, pfxPassword);
        File.WriteAllBytes(pfxPath, pfxBytes);

        // Assert
        Assert.NotNull(pfxBytes);
        //Assert.IsType<CertificateCreateDto>(result);
    } 
    [Fact]
    public void Inspect_Pfx()
    {
        //string certPath = Path.Combine("Certficate", "ca.cer"); // Path to your certificate file
        string certPath = "newCer.cer"; // Path to your certificate file
        //string keyPath = ""; // Path to your certificate file
        string keyPath = "newPrivateKey.private.key";  // Path to your private key file
        string pfxPath = Path.Combine("Certficate", "ca.pfx");          // Path where the PFX file will be saved
        string pfxPassword = "password";        // Password for the PFX file

        // Read the certificate
        var certBytes = File.ReadAllBytes(certPath);
        var certificate = new X509Certificate2(certBytes, pfxPassword);

        // Read the private key
        var privateKeyText = File.ReadAllText(keyPath);
        var privateKey = RSA.Create();
        privateKey.ImportFromPem(privateKeyText.ToCharArray());

        // Combine into an X509Certificate2 object with the private key
        var certWithKey = certificate.CopyWithPrivateKey(privateKey);
        var friendlyName = certWithKey.FriendlyName;
        var ThumbPrint = certWithKey.Thumbprint;
        var serialNumber = certWithKey.SerialNumber;
        var issuer = certWithKey.Issuer;
        // Export to PFX
        byte[] pfxBytes = certWithKey.Export(X509ContentType.Pfx, pfxPassword);
        File.WriteAllBytes(pfxPath, pfxBytes);

        // Assert
        Assert.NotNull(pfxBytes);
        //Assert.IsType<CertificateCreateDto>(result);
    }
}

