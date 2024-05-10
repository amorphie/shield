using amorphie.core.Base;
using amorphie.shield.core.Constant;
using amorphie.shield.core.Dto.Certificate;
using amorphie.shield.core.Mapper;
using amorphie.shield.core.Model;
using amorphie.shield.data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace amorphie.shield.app.Db;

public class CertificateService
{
    private readonly ShieldDbContext _dbContext;
    private readonly DbSet<Certificate> _dbSet;

    public CertificateService(ShieldDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Certificate>();
    }
    public async Task<Response<CertificateCreateResponseDto>> SaveAsync(CertificateCreateRequestDto certificateCreateRequestDto, X509Certificate2 x509Cert)
    {
        var certEntity = await _dbSet
            .FirstOrDefaultAsync(w => w.XDeviceId == certificateCreateRequestDto.XDeviceId);
        if (certEntity == null)
        {
            certEntity = CertificateMapper.Map(certificateCreateRequestDto, x509Cert);
            _dbSet.Add(certEntity);
            await _dbContext.SaveChangesAsync();
        }
        else if (certEntity.Status == CertificateStatus.Active)
        {
            return Response<CertificateCreateResponseDto>.Error("This device has a cert");
        }
        return Response<CertificateCreateResponseDto>.Success("");

    }
}

