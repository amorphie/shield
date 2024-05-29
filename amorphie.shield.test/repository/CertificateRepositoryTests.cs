using amorphie.shield.Certificates;
using amorphie.shield.test.Helpers;

namespace amorphie.shield.Repository;

public class CertificateRepositoryTests
{
    private readonly ShieldDbContext _context;
    private readonly CertificateRepository _repository;

    public CertificateRepositoryTests(ShieldDbContext context, CertificateRepository repository)
    {
        _context = context;
        _repository = repository;
    }

    [Fact]
    public async Task GetActiveAsync_Success()
    {
        var cert = CertificateHelper.CreateCertificateEntity();
        _context.Certificates.Add(cert);
        _context.SaveChanges();

        using var cts = new CancellationTokenSource();
        try
        {
            var result = await _repository.GetActiveAsync(cert.Id, cts.Token);
            Assert.NotNull(result);
        }
        catch (Exception e)
        {
            Assert.Fail("Unexpected exception" + e);
        }
    }
    [Fact]
    public async Task GetActiveAsync_Cancelled()
    {
        var guid = Guid.NewGuid();
        using var cts = new CancellationTokenSource();
        try
        {
            cts.Cancel();
            var result = await _repository.GetActiveAsync(guid, cts.Token);
            Assert.NotNull(result);
        }
        catch (Exception e)
        {
            Assert.IsType<OperationCanceledException>(e);
        }
    }
    [Fact]
    public async Task GetActiveAsync_Fail()
    {
        using var cts = new CancellationTokenSource();
        var guid = Guid.NewGuid();
        try
        {
            var result = await _repository.GetActiveAsync(guid, cts.Token);
        }
        catch (Exception e)
        {
            Assert.IsType<EntityNotFoundException>(e);
        }
    }
}

