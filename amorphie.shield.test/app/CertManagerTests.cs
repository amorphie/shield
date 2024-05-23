using System.Security.Cryptography.X509Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.Extension;


namespace amorphie.shield.test;
public class CertManagerTests
{
    private readonly string password = "password";

    [Fact]
    public void Create_ReturnsCert_WhenCertCreated()
    {
        var caFileName = "Certficate\\ca.pfx";
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
        ICaManager caManager = new FileCaManager();
        var certManager = new CertificateManager(caManager);
        var result = await certManager.CreateAsync("", "testCert", password);

        result.ExportPfx("clientCertPfx", password);
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

        var encResult = certManager.EncryptDataWithPublicKey(cerRSAPublicKey, dataTobeEnc);

        var encBytes = Convert.FromBase64String(encResult);

        var decryptResult = certManager.DecryptDataWithPrivateKey(encBytes, cerPrivateKey);

        var signedData = certManager.SignDataWithRSA(decryptResult, cerPrivateKey);
        var verify = certManager.Verify(decryptResult, signedData, cerRSAPublicKey);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<X509Certificate2>(result);
    }

}







