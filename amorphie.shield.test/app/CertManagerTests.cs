using System.Security.Cryptography.X509Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.Extension;
using amorphie.shield.test.Helpers;


namespace amorphie.shield.test;
public class CertManagerTests
{
    private readonly string password = "password";

    [Fact]
    public void Create_ReturnsCert_WhenCertCreated()
    {

        ICaManager caManager = new FileCaManager();
        var certManager = new CertificateManager(caManager);
        var result = certManager.CreateAsync("", "testCert", password);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<X509Certificate2>(result);
    }

    [Fact]
    public async Task Create_GetCaFromProvider_Then_CreateCertAsync()
    {

        var caBasePath = Path.Combine("c:\\cert\\client\\");
        var name = "client";
        ICaManager caManager = new FileCaManager();
        var certManager = new CertificateManager(caManager);
        var result = await certManager.CreateAsync("aclientofburgan", "testCert", password);


        var caCerPem = result.ExportCertificatePem();
        var caPfx = result.Export(X509ContentType.Pkcs12, password);
        await File.WriteAllBytesAsync($"{caBasePath}{name}.pfx", caPfx);
        await File.WriteAllTextAsync($"{caBasePath}{name}.cer", caCerPem);

        var privateKey = result.GetRSAPrivateKey()?.ExportPrivateKey();
        await File.WriteAllTextAsync($"{caBasePath}{name}.private.key", privateKey);

        var publicKey = result.GetRSAPrivateKey()?.ExportSubjectPublicKeyInfoPem();
        //var iseq = pk.Equals(publicKey);
        await File.WriteAllTextAsync($"{caBasePath}{name}.public.key", publicKey);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<X509Certificate2>(result);
    }
    [Fact]
    public async Task Enc_Dec_Sign_Verify_With_NewCert()
    {
        ICaManager caManager = new FileCaManager();
        var certManager = new CertificateManager(caManager);
        var result = await certManager.CreateAsync("", "testCert", password);
        var cerRSAPublicKey = certManager.GetRSAPublicKeyFromCertificate(result);
        var cerPrivateKey = result.GetRSAPrivateKey().ExportRSAPrivateKeyPem();
        var dataTobeEnc = "ibra";

        var encResult = CertificateManager.EncryptDataWithPublicKey(cerRSAPublicKey, dataTobeEnc);

        var encBytes = Convert.FromBase64String(encResult);

        var decryptResult = CertificateManager.DecryptDataWithPrivateKey(encBytes, cerPrivateKey);

        var signedData = CertificateManager.SignDataWithRSA(decryptResult, cerPrivateKey);
        var verify = CertificateManager.Verify(decryptResult, signedData, cerRSAPublicKey);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<X509Certificate2>(result);
    }

    [Fact]
    public async Task Enc_Dec_Sign_Verify_With_ExistingCert()
    {
        var cerRSAPublicKey = CertificateHelper.GetClientPublicKeyFromFile();

        var cerPrivateKey = CertificateHelper.GetClientPrivateKeyFromFile();
        var dataTobeEnc = "ibra";

        var encResult = CertificateManager.EncryptDataWithPublicKey(cerRSAPublicKey, dataTobeEnc);

        var encBytes = Convert.FromBase64String(encResult);

        var decryptResult = CertificateManager.DecryptDataWithPrivateKey(encBytes, cerPrivateKey);

        var signedData = CertificateManager.SignDataWithRSA(decryptResult, cerPrivateKey);
        var verify = CertificateManager.Verify(decryptResult, signedData, cerRSAPublicKey);
        // Assert
        Assert.NotNull(decryptResult);
        Assert.Equal(dataTobeEnc, decryptResult);
        Assert.True(verify);
    }

}







