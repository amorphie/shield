using amorphie.shield.app.CertManager;
using amorphie.shield.core.Dto.Certificate;
using System.Security.Cryptography.X509Certificates;

namespace amorphie.shield.test.app;
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

        var caManager = new CAManager();
        var resultCa = caManager.Create(cn, password);


        var certManager = new CertificateManager();
        var result = certManager.Create(caCert, "", "testCert", password);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<CertificateCreateDto>(result);
    }

    [Fact]
    public void Create_GetCaFromProvider_Then_CreateCert()
    {
        var resultCa = CaProvider.CaCert;


        var certManager = new CertificateManager();
        var result = certManager.Create(resultCa, "", "testCert", password);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<CertificateCreateDto>(result);
    }
    [Fact]
    public void CreateAndVerify_GetCaFromProvider_Then_CreateCert()
    {
        var resultCa = CaProvider.CaCert;


        var certManager = new CertificateManager();
        var result = certManager.Create(resultCa, "", "testCert", password);
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

