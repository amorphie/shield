using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using amorphie.shield.Extension;

namespace amorphie.shield.CertManager;
public class CertificateManager
{
    public X509Certificate2 Create(X509Certificate2 caCert, string identifier, string certName, string password)
    {
        // Create a new RSA provider for the client certificate
        // using (var rsaProvider = RSA.Create(4096))

        string certCN = $"{caCert.CN()}";
        if (!string.IsNullOrEmpty(identifier))
            certCN = $"{identifier}.{certCN}";

        using (var rsaProvider = RSA.Create(4096))
        {
            // Create a new certificate request for the client certificate
            var request = new CertificateRequest(
                new X500DistinguishedName(
                    $"CN={certCN}, OU=Digital Banking, O=Burgan Bank Turkey, C=TR"),
                rsaProvider,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            var keyUsage = new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true  // Add NonRepudiation if needed
            );

            request.CertificateExtensions.Add(keyUsage);

            var serialNumber = GenerateSerialNumber();
            // BigInteger a = new BigInteger(serialNumber);

            // Use the CA certificate to sign the client certificate request
            // var caCert = new X509Certificate2("ca.pfx", "password");
            var certificate = request.Create(caCert, DateTimeOffset.Now.AddMinutes(-1), DateTimeOffset.Now.AddYears(1), serialNumber);

            certificate = certificate.CopyWithPrivateKey(rsaProvider);

            return certificate;
        }
    }

    public string Encrypt(string publicPhase, string payloadData)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            byte[] dataToEncrypt = System.Text.Encoding.UTF8.GetBytes(payloadData);
            rsa.ImportParameters(ConvertPublicKeyStringToRSAParameters(publicPhase));
            return  Convert.ToBase64String(rsa.Encrypt(dataToEncrypt, false));
        }
    }

    private RSAParameters ConvertPublicKeyStringToRSAParameters(string publicKeyString)
    {
        byte[] publicKeyBytes = Convert.FromBase64String(publicKeyString);
        RSAParameters publicKey = new RSAParameters();
        using (var stream = new System.IO.MemoryStream(publicKeyBytes))
        {
            var reader = new System.IO.BinaryReader(stream);
            publicKey.Modulus = reader.ReadBytes(reader.ReadInt32());
            publicKey.Exponent = reader.ReadBytes(reader.ReadInt32());
        }
        return publicKey;
    }

    // Generate a new serial number for a certificate
    byte[] GenerateSerialNumber()
    {
        // Create a new RNGCryptoServiceProvider to generate random bytes
        using (var rng = new RNGCryptoServiceProvider())
        {
            // Create a byte array with the size of a certificate serial number (20 bytes)
            var serialNumber = new byte[20];

            // Fill the byte array with random values
            rng.GetBytes(serialNumber);

            // Return the generated serial number
            return serialNumber;
        }
    }
}

