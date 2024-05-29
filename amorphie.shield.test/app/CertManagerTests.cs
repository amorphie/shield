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
        var caFileName = "c:\\cert\\ca\\ca.pfx";
        X509Certificate2 caCert = new X509Certificate2(caFileName, password);
        var hpk = caCert.HasPrivateKey;
        var cn = "dev.ca.burganbank";

        //var caManager = new CaManager();
        //var resultCa = caManager.Create(cn, password);
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

        //result.ExportPfx("clientCertPfx", password);

        var caCerPem = result.ExportCertificatePem();
        var caPfx = result.Export(X509ContentType.Pkcs12, password);
        File.WriteAllBytes($"{caBasePath}{name}.pfx", caPfx);
        File.WriteAllText($"{caBasePath}{name}.cer", caCerPem);

        var privateKey = result.GetRSAPrivateKey()?.ExportPrivateKey();
        File.WriteAllText($"{caBasePath}{name}.private.key", privateKey);

        //var pk = Convert.ToBase64String(result.GetRSAPublicKey().ExportSubjectPublicKeyInfoPem());


        var publicKey = result.GetRSAPrivateKey()?.ExportSubjectPublicKeyInfoPem();
        //var iseq = pk.Equals(publicKey);
        File.WriteAllText($"{caBasePath}{name}.public.key", publicKey);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<X509Certificate2>(result);
    }
    [Fact]
    public async Task CreateAndVerify_Then_CreateCertAsync()
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
    public async Task Enc_Dec_Sign_Verify_WithExistingCert()
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
        //Assert.NotNull(result);
        //Assert.IsType<X509Certificate2>(result);
    }

}







