using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Numerics;

namespace amorphie.shield.Extension;
public static class CertExtensions
{
    public static BigInteger SerialNumber(this X509Certificate2 cert)
    {
        return new BigInteger(cert.GetSerialNumber());
    }
    public static string CN(this X509Certificate2 cert)
    {
        string pattern = @"CN=([^,]+)";

        Match match = Regex.Match(cert.Subject, pattern);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        else
        {
            return string.Empty;
        }
    }
    public static string Identifier(this X509Certificate2 cert)
    {
        string text = cert.CN();
        string pattern = @"^([^.]+)";
        Match match = Regex.Match(text, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        else
        {
            return string.Empty;
        }
    }

    public static void ExportPfx(this X509Certificate2 cert, string name, string password)
    {
        // Export the client certificate to a PFX file
        var clientCertBytes = cert.Export(X509ContentType.Pkcs12, password);

        // Save the client certificate to a file
        File.WriteAllBytes($"{name}.pfx", clientCertBytes);
    }

    public static string ExportCer(this X509Certificate2 cert)
    {
        return cert.ExportCertificatePem();
    }
    public static void ExportCer(this X509Certificate2 cert, string name)
    {
        var certPem = cert.ExportCer();
        File.WriteAllText($"{name}.cer", certPem);
    }

    public static void ExportPrivateKey(this RSACryptoServiceProvider rsa, string name)
    {
        var privateKeyPem = rsa.ExportPrivateKey();

        File.WriteAllText($"{name}.private_raw.key", Convert.ToBase64String(rsa.ExportRSAPrivateKey(), Base64FormattingOptions.None));
        File.WriteAllText($"{name}.private.key", privateKeyPem);
    }

    public static string ExportPrivateKey(this RSACryptoServiceProvider rsa)
    {
        var rsaPrivateKey = rsa.ExportRSAPrivateKey();
        var privateKeyPem = "-----BEGIN RSA PRIVATE KEY-----\r\n" +
                     Convert.ToBase64String(rsaPrivateKey, Base64FormattingOptions.InsertLineBreaks) +
                     "\r\n-----END RSA PRIVATE KEY-----";

        return privateKeyPem;
    }
    public static string ExportPrivateKey(this RSA rsa)
    {
        var rsaPrivateKey = rsa.ExportRSAPrivateKey();
        var privateKeyPem = "-----BEGIN RSA PRIVATE KEY-----\r\n" +
                     Convert.ToBase64String(rsaPrivateKey, Base64FormattingOptions.InsertLineBreaks) +
                     "\r\n-----END RSA PRIVATE KEY-----";

        return privateKeyPem;
    }


    public static void ExportPrivateKey(this RSA rsa, string name)
    {
        var privateKeyPem = rsa.ExportPrivateKey();

        File.WriteAllText($"{name}.private_raw.key", Convert.ToBase64String(rsa.ExportRSAPrivateKey(), Base64FormattingOptions.None));
        File.WriteAllText($"{name}.private.key", privateKeyPem);
    }

    public static void ExportPublicKey(this RSACryptoServiceProvider rsa, string name)
    {
        // Export the public key in PEM format
        var publicKeyPem = rsa.ExportPublicKey();
        File.WriteAllText($"{name}.public.key", publicKeyPem);
    }
    public static string ExportPublicKey(this RSACryptoServiceProvider rsa)
    {
        // Export the public key in PEM format
        var rsaPublicKey = rsa.ExportRSAPublicKey();
        var publicKeyPem = "-----BEGIN PUBLIC KEY-----\r\n" +
                           Convert.ToBase64String(rsaPublicKey, Base64FormattingOptions.InsertLineBreaks) +
                           "\r\n-----END PUBLIC KEY-----";

        return publicKeyPem;
    }
}
