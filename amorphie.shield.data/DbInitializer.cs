using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using amorphie.shield.core.Model;

namespace amorphie.shield.data;

public static class DbInitializer
{
    public static void Initialize(ShieldDbContext context)
    {
        context.Database.EnsureCreated();

        // Look for any students.
        if (context.Certificates.Any())
        {
            return; // DB has been seeded
        }
        // Adding 20 Certificate objects to the list
        for (int i = 1; i <= 20; i++)
        {
            var certificate = new Certificate
            {
                PublicCert = $"Certificate {i} Data",
                SerialNumber = $"Serial Number {i}",
                Status = i % 2 == 0 ? "Active" : "Passive",
                RevocationDate = DateTime.UtcNow.AddDays(i),
                ExpirationDate = DateTime.UtcNow.AddDays(i * 10),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid(),
                ModifiedAt = DateTime.UtcNow,
                ModifiedBy = Guid.NewGuid()
            };
            context.Certificates.Add(certificate);
        }

        context.SaveChanges();


    }
}
