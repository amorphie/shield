using amorphie.core.Base;
using Microsoft.EntityFrameworkCore;
using amorphie.shield.CertManager;
using amorphie.shield.Extension;

namespace amorphie.shield.Certificates;

public class CertificateAppService : ICertificateAppService
{
    private readonly CertificateRepository _certificateRepository;
    private readonly DbSet<Certificate> _dbSet;
    private readonly CertificateManager _certificateManager;

    public CertificateAppService(
        ShieldDbContext dbContext,
        CertificateRepository certificateRepository,
        CertificateManager certificateManager)
    {
        _dbSet = dbContext.Set<Certificate>();
        _certificateRepository = certificateRepository;
        _certificateManager = certificateManager;
    }

    public async Task<Response<CertificateCreateResponseDto>> CreateAsync(CertificateCreateRequestDto input)
    {
        var caCertificate = _certificateManager.Create(
            CaProvider.CaCert,
            input.Identity.UserTCKN.ToString(),
            "testClient",
            "password");
        
        var certEntity = await _dbSet
            .FirstOrDefaultAsync(w => w.Identity.DeviceId == input.Identity.DeviceId);
        if (certEntity == null)
        {
            certEntity = new Certificate(
                cn: caCertificate.CN(),
                deviceId: input.Identity.DeviceId,
                tokenId: input.Identity.TokenId,
                requestId: input.Identity.RequestId,
                userTckn: input.Identity.UserTCKN.ToString(),
                instanceId: input.InstanceId,
                serialNumber: caCertificate.SerialNumber().ToString(),
                publicCert: caCertificate.ExportCer(),
                thumbprint: caCertificate.Thumbprint,
                expirationDate: caCertificate.NotAfter.ToUniversalTime()
            );
            
            certEntity.Active();
            await _certificateRepository.InsertAsync(certEntity);
        }
        else if (certEntity.Status == CertificateStatus.Active)
        {
            return Response<CertificateCreateResponseDto>.Error("This device has a cert");
        }

        return Response<CertificateCreateResponseDto>.Success("");
    }
}
