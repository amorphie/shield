using amorphie.shield.data;
using Microsoft.EntityFrameworkCore;

namespace amorphie.shield.Certificates
{
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
            .FirstOrDefaultAsync(p => p.Identity.DeviceId == deviceId && p.Status == (int)CertificateStatus.Active,
            cancellationToken);
        }

        public async Task<Certificate> InsertAsync(Certificate certificate,
            CancellationToken cancellationToken = default)
        {
            _dbSet.Add(certificate);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return certificate;
        }
    }
}
