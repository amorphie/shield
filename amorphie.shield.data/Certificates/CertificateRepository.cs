using Microsoft.EntityFrameworkCore;

namespace amorphie.shield.Certificates;

public class CertificateRepository
{
    private readonly ShieldDbContext _dbContext;
    private readonly DbSet<Certificate> _dbSet;

    public CertificateRepository(ShieldDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Certificate>();
    }

    public async Task<Certificate> FindByDeviceActiveAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstAsync(p => p.Identity.DeviceId == deviceId && p.Status == CertificateStatus.Active,
                cancellationToken);
    }
    
    public async Task<Certificate> GetActiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var certificate =  await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.Status == CertificateStatus.Active,
                cancellationToken);

        if (certificate != null)
        {
            return certificate;
        }

        throw new EntityNotFoundException(typeof(Certificate), id, "Certificate not found");
    }
    
    public async Task<Certificate> GetByTokenAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        var certificate =  await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Identity.TokenId == tokenId && p.Status == CertificateStatus.Active,
                cancellationToken);

        if (certificate != null)
        {
            return certificate;
        }

        throw new EntityNotFoundException(typeof(Certificate), tokenId, "Token certificate not found");
    }
    
    public async Task<Certificate> GetByDeviceAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        var certificate =  await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Identity.DeviceId == deviceId && p.Status == CertificateStatus.Active,
                cancellationToken);

        if (certificate != null)
        {
            return certificate;
        }

        throw new EntityNotFoundException(typeof(Certificate), deviceId, "Device certificate not found");
    }
    
    public async Task<Certificate> GetByUserAsync(string userTckn, CancellationToken cancellationToken = default)
    {
        var certificate =  await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Identity.UserTCKN.ToString() == userTckn && p.Status == CertificateStatus.Active,
                cancellationToken);

        if (certificate != null)
        {
            return certificate;
        }

        throw new EntityNotFoundException(typeof(Certificate), userTckn, "User certificate not found");
    }

    public async Task<Certificate> InsertAsync(Certificate certificate,
        CancellationToken cancellationToken = default)
    {
        _dbSet.Add(certificate);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return certificate;
    }
    
    public async Task<Certificate> UpdateAsync(Certificate certificate,
        CancellationToken cancellationToken = default)
    {
        _dbSet.Update(certificate);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return certificate;
    }
}