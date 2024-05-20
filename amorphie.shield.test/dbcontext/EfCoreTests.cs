using amorphie.shield.Certificates;

namespace amorphie.shield.dbcontext;
public class EfCoreTests
{
    private readonly ShieldDbContext _shieldDbContext;

    public EfCoreTests(ShieldDbContext shieldDbContext)
    {
        _shieldDbContext = shieldDbContext;
        DbInitializer.Initialize(_shieldDbContext);
    }
    [Fact]
    public void Get_Certificate()
    {
        var cert = _shieldDbContext.Certificates.FirstOrDefault();
        Assert.NotNull(cert);
        Assert.IsType<Certificate>(cert);
    }
    [Fact]
    public async Task Get_And_Update_CertificateAsync()
    {
        var cert = _shieldDbContext.Certificates.FirstOrDefault();
        Assert.NotNull(cert);

        cert.Passive("Reason is test");

        _shieldDbContext.Certificates.Update(cert);
        var savingResult = await _shieldDbContext.SaveChangesAsync();
        Assert.True(savingResult > 0);
    }
}

