using amorphie.shield.Certificates;

namespace amorphie.shield.app;
public class CertificationAppServiceTest
{
    private readonly ICertificateAppService _certificateAppService;

    public CertificationAppServiceTest(ICertificateAppService certificateAppService)
    {
        _certificateAppService = certificateAppService;
    }

    [Fact]
    public async Task CreateAsync_Success()
    {
        var certificate = await _certificateAppService.CreateAsync(new CertificateCreateInputDto()
        {
            InstanceId = Guid.NewGuid(),
            Identity = new Shared.IdentityDto()
            {
                DeviceId = AppConsts.DeviceId,
                RequestId = Guid.NewGuid(),
                TokenId = Guid.NewGuid(),
                UserTCKN = AppConsts.UserTckn
            }
        });

        Assert.Equal("Success", certificate.Result.Status);
    }
    [Fact]
    public async Task Assert_Create_And_Get_Async()
    {
        await CreateAsync_Success();
        var certificate = await _certificateAppService.GetByDeviceIdAsync(AppConsts.DeviceId);

        Assert.Equal("Success", certificate.Result.Status);
    }
    [Fact]
    public async Task GetByDeviceIdAsync_Fails()
    {
        await CreateAsync_Success();
        var certificate = await _certificateAppService.GetByDeviceIdAsync(AppConsts.DeviceId);

        Assert.Equal("Success", certificate.Result.Status);
    }
}
