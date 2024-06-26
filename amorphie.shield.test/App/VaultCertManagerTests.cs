using System.Security.Cryptography.X509Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.test.Helpers;
using VaultSharp;


namespace amorphie.shield.test;
public class VaultCertManagerTests
{
    private readonly string password = "password";
    private readonly string certName = "vaultservercert";

    private readonly IVaultClient _vaultClient;
    private readonly Microsoft.Extensions.Options.IOptions<VaultOptions> _options;


    public VaultCertManagerTests(IVaultClient vaultClient, Microsoft.Extensions.Options.IOptions<VaultOptions> options)
    {
        _vaultClient = vaultClient;
        _options = options;
    }

    [Fact]
    public async Task Create_CreateCertAsync()
    {
        var certBasePath = Path.Combine("c:\\cert\\client\\");

        var certManager = new VaultCertificateManager(_vaultClient, _options);
        var result = await certManager.CreateAsync("2");

        var certPem = result.ExportCertificatePem();
        var certPfx = result.Export(X509ContentType.Pkcs12, password);
        await File.WriteAllBytesAsync($"{certBasePath}{certName}.pfx", certPfx);
        await File.WriteAllTextAsync($"{certBasePath}{certName}.cer", certPem);

        var privateKey = result.GetRSAPrivateKey()?.ExportRSAPrivateKeyPem();
        await File.WriteAllTextAsync($"{certBasePath}{certName}.private.key", privateKey);

        var publicKey = result.GetRSAPrivateKey()?.ExportSubjectPublicKeyInfoPem();
        await File.WriteAllTextAsync($"{certBasePath}{certName}.public.key", publicKey);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<X509Certificate2>(result);
    }

    [Fact]
    public void Generate_Vault_Pfx_From_Cer_And_PrivateKey()
    {
        // Read the certificate
        var certBytes = CertificateHelper.GetVaultCaCertFromFile();
        var certificate = new X509Certificate2(certBytes, StaticData.Password);

        // Read the private key
        var privateKey = CertificateHelper.GetVaultCaPrivateKeyFromFile_RSA();

        // Combine into an X509Certificate2 object with the private key
        var certWithKey = certificate.CopyWithPrivateKey(privateKey);

        // Export to PFX
        byte[] pfxBytes = certWithKey.Export(X509ContentType.Pfx, StaticData.Password);
        File.WriteAllBytes(Path.Combine(StaticData.CaCertBasePath, "vaultca.pfx"), pfxBytes);

        // Assert
        Assert.NotNull(pfxBytes);
    }

    [Fact]
    public void Enc_Dec_Sign_Verify_Using_ExistingCert()
    {
        var cerRSAPublicKey = CertificateHelper.GetClientPublicKeyFromFile(certName);

        var cerPrivateKey = CertificateHelper.GetClientPrivateKeyFromFile(certName);
        var dataTobeEnc = "ibra";

        var encResult = CertificateUtil.EncryptDataWithPublicKey(cerRSAPublicKey, dataTobeEnc);

        var encBytes = Convert.FromBase64String(encResult);

        var decryptResult = CertificateUtil.DecryptDataWithPrivateKey(cerPrivateKey, encBytes);

        var signedData = CertificateUtil.SignDataWithRSA(cerPrivateKey, decryptResult);
        var verify = CertificateUtil.Verify(cerRSAPublicKey, decryptResult, signedData);
        // Assert
        Assert.NotNull(decryptResult);
        Assert.Equal(dataTobeEnc, decryptResult);
        Assert.True(verify);
    }


    //This suppose to not work
    [Fact(DisplayName = "Reverse_Encryption_Decryption This suppose to now work")]
    public async Task EncWithPrivate_DecWithPublic_Using_ExistingCert()
    {
        var cerRSAPublicKey = CertificateHelper.GetClientPublicKeyFromFile(certName);

        var cerPrivateKey = CertificateHelper.GetClientPrivateKeyFromFile(certName);
        var dataTobeEnc = "ibra";

        var encResult = CertificateUtil.EncryptDataWithPublicKey(cerPrivateKey, dataTobeEnc);

        var encBytes = Convert.FromBase64String(encResult);

        var decryptResult = CertificateUtil.DecryptDataWithPrivateKey(cerRSAPublicKey, encBytes);

        // Assert
        Assert.NotNull(decryptResult);
        Assert.Equal(dataTobeEnc, decryptResult);
    }

}







