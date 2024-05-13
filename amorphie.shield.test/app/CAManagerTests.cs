using amorphie.shield.Certificates;
using amorphie.shield.CertManager;

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
        // Assert
        Assert.NotNull(result);
        Assert.IsType<CertificateCreateDto>(result);
    }
}

