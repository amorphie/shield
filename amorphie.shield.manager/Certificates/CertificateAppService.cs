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

    public async Task<Response<CertificateCreateOutputDto>> CreateAsync(CertificateCreateInputDto input)
    {
        var caCertificate = _certificateManager.Create(
            CaProvider.CaCert,
            input.Identity.UserTCKN.ToString(),
            "testClient",
            "password");

        var certEntity = new Certificate(
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


        return Response<CertificateCreateOutputDto>.Success("");
    }

    public async Task<Response<CertificateQueryOutputDto>> GetBySerialNumberAsync(string serialNumber)
    {
        var certEntity = await _dbSet.FirstOrDefaultAsync(w => w.SerialNumber == serialNumber);
        return Get(certEntity);
    }
    public async Task<Response<CertificateQueryOutputDto>> GetByDeviceIdAsync(Guid deviceId)
    {
        var certEntity = await _dbSet.FirstOrDefaultAsync(w => w.Identity.DeviceId == deviceId);
        return Get(certEntity);
    }
    private static Response<CertificateQueryOutputDto> Get(Certificate? certEntity)
    {
        if (certEntity == null)
        {
            throw new BusinessException(code: 404, severity: "warning", message: "Not exist", details: "User certificate not found");
        }
        else if (certEntity.Status != CertificateStatus.Active)
        {
            throw new BusinessException(code: 404, severity: "warning", message: "Not active", details: "User certificate is not active");

        }
        else if (certEntity.Status == CertificateStatus.Active)
        {
            return Response<CertificateQueryOutputDto>.Success("success", new CertificateQueryOutputDto { Valid = true, ExpirationDate = certEntity.ExpirationDate });
        }

        throw new BusinessException(code: 500, severity: "warning", message: "Business Error", details: "Unproccesable status for user certificate");
    }
}
