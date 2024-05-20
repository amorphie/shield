using amorphie.shield.Certificates;

namespace amorphie.shield.Helpers;

public static class CertificateHelper
{
    public static Certificate CreateCertificateEntity()
    {
        return new Certificate(
        cn: $"34987491780.dev.ca.burganbank",
        deviceId: "27abe79b-b2fa-4d3e-b656-61ce2fdb2a07",
        tokenId: Guid.Parse("18ab1b9f-6dbb-45de-a197-7b6f5a152503"),
        requestId: Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
        userTckn: "34000491799",
        instanceId: Guid.NewGuid(),
        serialNumber: "54564564545",
        publicCert: "5456464564564",
        thumbprint: "5645465456456",
        expirationDate: DateTime.UtcNow.AddDays(1 * 10)
        )
        {
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = Guid.NewGuid()
        };
    }
}

