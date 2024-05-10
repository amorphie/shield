using amorphie.shield.app.CertManager;
using amorphie.shield.core.Dto.Certificate;
using amorphie.shield.core.Dto.Certificate;

namespace amorphie.shield.test.app;
public class CAManagerTests
{
    [Fact]
    public void Create_RetursCaCreateDto_WhenCaCreated()
    {
        var cn = "dev.ca.burganbank";
        var password = "password";

        var caManager = new CAManager();
        var result = caManager.Create(cn, password);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<CertificateCreateDto>(result);
    }
}

