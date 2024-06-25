using amorphie.shield.Certificates;
using System.Security.Cryptography;

namespace amorphie.shield.test.Helpers;

public static class CertificateHelper
{
    public static Certificate CreateCertificateEntity()
    {
        return new Certificate(
        cn: $"12345678900.dev.ca.burganbank",
        deviceId: "27abe79b-b2fa-4d3e-b656-61ce2fdb2a07",
        tokenId: Guid.Parse("18ab1b9f-6dbb-45de-a197-7b6f5a152503"),
        requestId: Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
        userTckn: "34000491799",
        instanceId: Guid.NewGuid(),
        serialNumber: "54564564545",
        publicCert: "5456464564564",
        thumbprint: "5645465456456",
        expirationDate: DateTime.UtcNow.AddDays(1 * 10),
        origin: CertificateOrigin.Server
        )
        {
            CreatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = Guid.NewGuid()
        };
    }

    #region Client Read From File

    public static RSA GetClientPrivateKeyFromFile_RSA(string certName)
    {
        var privateKeyPath = Path.Combine(StaticData.ClientCertBasePath, $"{certName}.private.key");
        var privateKeyString = File.ReadAllText(privateKeyPath);
        var privateKey = RSA.Create();
        privateKey.ImportFromPem(privateKeyString.ToCharArray());
        return privateKey;
    }
    public static string GetClientPrivateKeyFromFile(string certName)
    {
        var privateKeyPath = Path.Combine(StaticData.ClientCertBasePath, $"{certName}.private.key");
        return File.ReadAllText(privateKeyPath);

    }
    public static byte[] GetClientPfxFromFile(string certName)
    {
        var privateKeyPath = Path.Combine(StaticData.ClientCertBasePath, $"{certName}.pfx");
        return File.ReadAllBytes(privateKeyPath);

    }
    public static byte[] GetClientCertFromFile(string certName)
    {
        var privateKeyPath = Path.Combine(StaticData.ClientCertBasePath, $"{certName}.cer");
        return File.ReadAllBytes(privateKeyPath);

    }
    #endregion

    public static string GetClientPublicKeyFromFile(string certName)
    {
        var privateKeyPath = Path.Combine(StaticData.ClientCertBasePath, $"{certName}.public.key");
        return File.ReadAllText(privateKeyPath);

    }

    #region Ca Read From File
    public static RSA GetCaPrivateKeyFromFile_RSA()
    {
        var privateKeyPath = Path.Combine(StaticData.CaCertBasePath, "ca.private.key");
        var privateKeyString = File.ReadAllText(privateKeyPath);
        var privateKey = RSA.Create();
        privateKey.ImportFromPem(privateKeyString.ToCharArray());
        return privateKey;
    }
    public static string GetCaPrivateKeyFromFile()
    {
        var privateKeyPath = Path.Combine(StaticData.CaCertBasePath, "ca.private.key");
        return File.ReadAllText(privateKeyPath);

    }
    public static byte[] GetCaPfxFromFile()
    {
        var privateKeyPath = Path.Combine(StaticData.CaCertBasePath, "ca.pfx");
        return File.ReadAllBytes(privateKeyPath);

    }
    public static byte[] GetCaCertFromFile()
    {
        var privateKeyPath = Path.Combine(StaticData.CaCertBasePath, "ca.cer");
        return File.ReadAllBytes(privateKeyPath);

    }

    public static string GetCaPublicKeyFromFile()
    {
        var privateKeyPath = Path.Combine(StaticData.CaCertBasePath, "ca.public.key");
        return File.ReadAllText(privateKeyPath);

    }
    #endregion

    #region Vault Ca Read From File
    public static RSA GetVaultCaPrivateKeyFromFile_RSA()
    {
        var privateKeyPath = Path.Combine(StaticData.CaCertBasePath, "vaultca.private.key");
        var privateKeyString = File.ReadAllText(privateKeyPath);
        var privateKey = RSA.Create();
        privateKey.ImportFromPem(privateKeyString.ToCharArray());
        return privateKey;
    }
    public static string GetVaultCaPrivateKeyFromFile()
    {
        var privateKeyPath = Path.Combine(StaticData.CaCertBasePath, "vaultca.private.key");
        return File.ReadAllText(privateKeyPath);

    }
    public static byte[] GetVaultCaPfxFromFile()
    {
        var privateKeyPath = Path.Combine(StaticData.CaCertBasePath, "vaultca.pfx");
        return File.ReadAllBytes(privateKeyPath);

    }
    public static byte[] GetVaultCaCertFromFile()
    {
        var privateKeyPath = Path.Combine(StaticData.CaCertBasePath, "vaultca.cer");
        return File.ReadAllBytes(privateKeyPath);

    }

    #endregion

    #region Free TSA Read From File
    public static byte[] GetTsaCaCertFromFile()
    {
        var privateKeyPath = Path.Combine(StaticData.CaCertBasePath, "tsaca.cer");
        return File.ReadAllBytes(privateKeyPath);

    }
        public static byte[] GetTsaCertFromFile()
    {
        var privateKeyPath = Path.Combine(StaticData.CaCertBasePath, "tsa.crt");
        return File.ReadAllBytes(privateKeyPath);

    }

    #endregion
}

