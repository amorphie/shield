using System.Security.Cryptography.X509Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.Extension;
using amorphie.shield.test.Helpers;


namespace amorphie.shield.test;
public class CertManagerTests
{
    private readonly string password = "password";
    private readonly string certName = "subclient_vault";


    [Fact]
    public void Create_ReturnsCert_WhenCertCreated()
    {

        ICaManager caManager = new FileCaManager();
        var certManager = new CertificateManager(caManager);
        var result = certManager.CreateAsync("");
        // Assert
        Assert.NotNull(result);
        Assert.IsType<X509Certificate2>(result);
    }

    [Fact]
    public async Task Create_GetCaFromProvider_Then_CreateCertAsync()
    {

        var caBasePath = Path.Combine("c:\\cert\\client\\");
        ICaManager caManager = new FileCaManager();
        var certManager = new CertificateManager(caManager);
        var result = await certManager.CreateAsync("AClientOfBurgan");


        var caCerPem = result.ExportCertificatePem();
        var caPfx = result.Export(X509ContentType.Pkcs12, password);
        await File.WriteAllBytesAsync($"{caBasePath}{certName}.pfx", caPfx);
        await File.WriteAllTextAsync($"{caBasePath}{certName}.cer", caCerPem);

        var privateKey = result.GetRSAPrivateKey()?.ExportPrivateKey();
        await File.WriteAllTextAsync($"{caBasePath}{certName}.private.key", privateKey);

        var publicKey = result.GetRSAPrivateKey()?.ExportSubjectPublicKeyInfoPem();
        //var iseq = pk.Equals(publicKey);
        await File.WriteAllTextAsync($"{caBasePath}{certName}.public.key", publicKey);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<X509Certificate2>(result);
    }
    [Fact]
    public async Task Enc_Dec_Sign_Verify_Using_NewGeneratedCert()
    {
        ICaManager caManager = new FileCaManager();
        var certManager = new CertificateManager(caManager);
        var result = await certManager.CreateAsync("");
        var cerRSAPublicKey = CertificateUtil.GetRSAPublicKeyFromCertificate(result);
        var cerPrivateKey = result.GetRSAPrivateKey().ExportRSAPrivateKeyPem();
        var dataTobeEnc = "ibra";

        var encResult = CertificateUtil.EncryptDataWithPublicKey(cerRSAPublicKey, dataTobeEnc);

        var encBytes = Convert.FromBase64String(encResult);

        var decryptResult = CertificateUtil.DecryptDataWithPrivateKey(cerPrivateKey, encBytes);

        var signedData = CertificateUtil.SignDataWithRSA(cerPrivateKey, decryptResult);
        var verify = CertificateUtil.Verify(cerRSAPublicKey, decryptResult, signedData);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<X509Certificate2>(result);
    }

    [Fact]
    public async Task Enc_Dec_Sign_Verify_Using_ExistingCert()
    {
        var cerRSAPublicKey = CertificateHelper.GetClientPublicKeyFromFile(certName);

        var cerPrivateKey = CertificateHelper.GetClientPrivateKeyFromFile(certName);
        var dataTobeEnc = "ibra";

        var encResult = CertificateUtil.EncryptDataWithPublicKey(cerRSAPublicKey, dataTobeEnc);

        var encBytes = Convert.FromBase64String(encResult);

        var decryptResult = CertificateUtil.DecryptDataWithPrivateKey(cerPrivateKey, encBytes);

        var signedData = CertificateUtil.SignDataWithRSA(decryptResult, cerPrivateKey);
        var verify = CertificateUtil.Verify(cerRSAPublicKey,decryptResult, signedData);
        // Assert
        Assert.NotNull(decryptResult);
        Assert.Equal(dataTobeEnc, decryptResult);
        Assert.True(verify);
    }


}







