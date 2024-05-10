using amorphie.shield.app.CertManager;
using amorphie.shield.core.Dto.Certificate;

namespace amorphie.shield.test.app;
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

