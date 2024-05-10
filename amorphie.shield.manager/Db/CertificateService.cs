using amorphie.core.Base;
using amorphie.shield.core.Dto.Certificate;
using amorphie.shield.core.Model;
using amorphie.shield.data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public async Task<Response> SaveAsync(CertificateCreateDto data)
    {
        var existingRecord = await _dbSet
            .FirstOrDefaultAsync(w => w.Id == data.Id);
        if (existingRecord == null)
        {
            // var cert = new Certificate
            // {
            //     Id = data.Id,
            //     PublicCert = data.PublicCert
            // };
        }
        else
        {
        }
        return Response.Success("");

    }
}

