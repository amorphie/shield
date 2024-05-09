using amorphie.shield.app.CertManager;
using amorphie.shield.core.Dto.CaDto;

namespace amorphie.shield.test.app;
public class CAManagerTests
{
    [Fact]
    public void Create_RetursCaCreateDto_WhenCaCreated()
    {
        var caManager = new CAManager();
        var result = caManager.Create();
        // Assert
        Assert.NotNull(result);
        Assert.IsType<CaCreateDto>(result);
    }
}

