using amorphie.core.Base;
using Microsoft.EntityFrameworkCore;
using amorphie.shield.CertManager;
using amorphie.shield.Extension;
using System.Security.Cryptography.X509Certificates;

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
        var certificate = _certificateManager.Create(
            input.Identity.UserTCKN.ToString(),
            "testClient",
            "password");

        var certEntity = new Certificate(
            cn: certificate.CN(),
            deviceId: input.Identity.DeviceId,
            tokenId: input.Identity.TokenId,
            requestId: input.Identity.RequestId,
            userTckn: input.Identity.UserTCKN.ToString(),
            instanceId: input.InstanceId,
            serialNumber: certificate.SerialNumber().ToString(),
            publicCert: certificate.ExportCer(),
            thumbprint: certificate.Thumbprint,
            expirationDate: certificate.NotAfter.ToUniversalTime()
        );

        certEntity.Active();
        await _certificateRepository.InsertAsync(certEntity);

        return Response<CertificateCreateOutputDto>.Success("", new CertificateCreateOutputDto
        {
            Id = certEntity.Id,
            Certificate= certificate.ExportCer(),
            PrivateKey=certificate.GetRSAPrivateKey()?.ExportPrivateKey(),
            ExpirationDate = certificate.NotAfter.ToUniversalTime()
        });
    }

    public async Task<Response<CertificateQueryOutputDto>> GetBySerialAsync(string serialNumber)
    {
        var certEntity = await _dbSet.FirstOrDefaultAsync(w => w.SerialNumber == serialNumber);
        return Get(certEntity);
    }
    public async Task<Response<CertificateQueryOutputDto>> GetBySerialAndUserTcknAsync(string serialNumber, string userTckn)
    {
        var certEntity = await _dbSet.FirstOrDefaultAsync(w => w.SerialNumber == serialNumber && w.Identity.UserTCKN == userTckn);
        return Get(certEntity);
    }
    public async Task<Response<CertificateQueryOutputDto>> GetBySerialAndUserTcknAndXTokenIdAsync(string serialNumber, string userTckn, Guid xTokenId)
    {
        var certEntity = await _dbSet.FirstOrDefaultAsync(w => w.SerialNumber == serialNumber && w.Identity.UserTCKN == userTckn && w.Identity.TokenId == xTokenId);
        return Get(certEntity);
    }

    public async Task<Response<CertificateQueryOutputDto>> GetByUserTcknAndXTokenIdAsync(string userTckn, Guid xTokenId)
    {
        var certEntity = await _dbSet.FirstOrDefaultAsync(w => w.Identity.UserTCKN == userTckn && w.Identity.TokenId == xTokenId);
        return Get(certEntity);
    }

    public async Task<Response<CertificateQueryOutputDto>> GetByUserTcknAndXDeviceIdAsync(string userTckn, Guid xDeviceId)
    {
        var certEntity = await _dbSet.FirstOrDefaultAsync(w => w.Identity.UserTCKN == userTckn && w.Identity.TokenId == xDeviceId);
        return Get(certEntity);
    }

    public async Task<Response<CertificateQueryOutputDto>> GetByDeviceIdAsync(Guid xDeviceId)
    {
        var certEntity = await _dbSet.FirstOrDefaultAsync(w => w.Identity.DeviceId == xDeviceId);
        var result = Get(certEntity);
        result.Data.Identity = new Shared.IdentityDto
        {
            DeviceId = certEntity!.Identity.DeviceId,
            TokenId=certEntity!.Identity.TokenId,
            RequestId=certEntity!.Identity.RequestId,
            UserTCKN=certEntity!.Identity.UserTCKN,
        };
        return result;
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
