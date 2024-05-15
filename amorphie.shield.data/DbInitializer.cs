using amorphie.shield.Certificates;

namespace amorphie.shield;

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
        // for (int i = 1; i <= 20; i++)
        // {
        var certificate = new Certificate(
            $"34987491780.dev.ca.burganbank",
            Guid.Parse("27abe79b-b2fa-4d3e-b656-61ce2fdb2a07"),
            Guid.Parse("18ab1b9f-6dbb-45de-a197-7b6f5a152503"),
            Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            "34987491780",
            Guid.NewGuid(),
            "54564564545",
            "5456464564564",
            "5645465456456",
            DateTime.UtcNow.AddDays(1 * 10)
        )
        {
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = Guid.NewGuid()
        };
        certificate.Active();
        context.Certificates.Add(certificate);
        // }

        context.SaveChanges();


    }
}
