using System.Security.Cryptography.X509Certificates;
using System.Text;
using amorphie.shield.Certificates;
using amorphie.shield.CertManager;

namespace amorphie.shield.app;
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
        var result = certManager.Create("", "testCert", password);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<CertificateCreateDto>(result);
    }

    [Fact]
    public void Create_GetCaFromProvider_Then_CreateCert()
    {
        ICaManager caManager = new FileCaManager();
        var certManager = new CertificateManager(caManager);
        var result = certManager.Create("", "testCert", password);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<CertificateCreateDto>(result);
    }
    [Fact]
    public void CreateAndVerify_GetCaFromProvider_Then_CreateCert()
    {
        ICaManager caManager = new FileCaManager();
        var certManager = new CertificateManager(caManager);
        var result = certManager.Create("", "testCert", password);
        var cerRSAPublicKey = certManager.GetRSAPublicKeyFromCertificate(result);
        var cerPrivateKey = result.GetRSAPrivateKey().ExportRSAPrivateKeyPem();
        var dataTobeEnc = "ibra";
        var encResult = certManager.EncryptDataWithPublicKey(cerRSAPublicKey, dataTobeEnc);

        var encBytes = Convert.FromBase64String(encResult);

        var decryptResult = certManager.DecryptDataWithPrivateKey(encBytes, cerPrivateKey);

        var signedData = certManager.SignDataWithRSA(decryptResult, cerPrivateKey);
        var verify = certManager.Verify(decryptResult, signedData, cerRSAPublicKey);
        //var signature = resultCa.GetSignature();
        //var tbs = result.Cert.GetTbsCertificate();
        //var alg = signed.SignatureAlgorithm;
        // Assert
        Assert.NotNull(result);
        Assert.IsType<CertificateCreateDto>(result);
    }

    public string ReadCaFromFile()
    {
        var caFile = "Certficate\\ca.cer";
        return File.ReadAllText(caFile);
    }
}

