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
        // Creating a list of Certificate objects
        List<Certificate> certificateList = new List<Certificate>();
        // Adding 20 Certificate objects to the list
        for (int i = 1; i <= 20; i++)
        {
            var certificate = new Certificate
            {
                CertificateData = $"Certificate {i} Data",
                PrivateKey = $"Encrypted Private Key {i}",
                Status = i % 2 == 0 ? "Active" : "Expired", // Alternate between "Active" and "Expired" statuses
                RevocationDate = i % 3 == 0 ? DateTime.Now.AddDays(-i) : DateTime.MinValue, // Revoked every third certificate
                ExpirationDate = DateTime.Now.AddDays(i * 10) // Expiration date incremented by 10 days for each certificate
            };
            context.Certificates.Add(certificate);
        }

        context.SaveChanges();


    }
}
