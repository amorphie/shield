using amorphie.shield.Certificates;
using amorphie.shield.Revokes;
using amorphie.shield.Shared;

namespace amorphie.shield.app;

public class RevokeAppServiceTest
{
    private readonly IRevokeAppService _revokeAppService;
    private readonly ICertificateAppService _certificateAppService;

    public RevokeAppServiceTest(
        IRevokeAppService revokeAppService,
        ICertificateAppService certificateAppService
        )
    {
        _revokeAppService = revokeAppService;
        _certificateAppService = certificateAppService;
    }

    [Fact]
    public async Task Assert_Revoke_Token_Async()
    {
        var mockTokenId = Guid.NewGuid();
        var certificateResponse = await _certificateAppService.CreateAsync(
            new CertificateCreateInputDto()
            {
                Identity = new IdentityDto()
                {
                    RequestId = Guid.NewGuid(),
                    DeviceId = Guid.NewGuid().ToString(),
                    TokenId = mockTokenId,
                    UserTCKN = "34981"
                },
                InstanceId = Guid.NewGuid()
            });
        
        Assert.Equal("Success", certificateResponse.Result.Status);

        var revokeResponse = await _revokeAppService.RevokeTokenAsync(mockTokenId);
        
        Assert.True(revokeResponse.Data.Revoked);
    }
    
    [Fact]
    public async Task Assert_Revoke_Device_Async()
    {
        var mockDeviceId = Guid.NewGuid();
        var certificateResponse = await _certificateAppService.CreateAsync(
            new CertificateCreateInputDto()
            {
                Identity = new IdentityDto()
                {
                    RequestId = Guid.NewGuid(),
                    DeviceId = mockDeviceId.ToString(),
                    TokenId = Guid.NewGuid(),
                    UserTCKN = "34981"
                },
                InstanceId = Guid.NewGuid()
            });
        
        Assert.Equal("Success", certificateResponse.Result.Status);

        var revokeResponse = await _revokeAppService.RevokeDeviceAsync(mockDeviceId.ToString());
        
        Assert.True(revokeResponse.Data.Revoked);
    }
    
    [Fact]
    public async Task Assert_Revoke_User_Async()
    {
        var mockTckn = "34987";
        var certificateResponse = await _certificateAppService.CreateAsync(
            new CertificateCreateInputDto()
            {
                Identity = new IdentityDto()
                {
                    RequestId = Guid.NewGuid(),
                    DeviceId = Guid.NewGuid().ToString(),
                    TokenId = Guid.NewGuid(),
                    UserTCKN = mockTckn
                },
                InstanceId = Guid.NewGuid()
            });
        
        Assert.Equal("Success", certificateResponse.Result.Status);

        var revokeResponse = await _revokeAppService.RevokeUserAsync(mockTckn);
        
        Assert.True(revokeResponse.Data.Revoked);
    }
    
    [Fact]
    public async Task Assert_Revoke_Certificate_Async()
    {
        var certificateResponse = await _certificateAppService.CreateAsync(
            new CertificateCreateInputDto()
            {
                Identity = new IdentityDto()
                {
                    RequestId = Guid.NewGuid(),
                    DeviceId = Guid.NewGuid().ToString(),
                    TokenId = Guid.NewGuid(),
                    UserTCKN = "34989"
                },
                InstanceId = Guid.NewGuid()
            });
        
        Assert.Equal("Success", certificateResponse.Result.Status);

        var revokeResponse = await _revokeAppService.RevokeCertificateAsync(certificateResponse.Data.Id);
        
        Assert.True(revokeResponse.Data.Revoked);
    }
}
