using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using amorphie.shield.Extension;

namespace amorphie.shield.CertManager;

public class CertificateManager
{
    private readonly ICaManager _caManager;

    public CertificateManager(ICaManager caManager)
    {
        _caManager = caManager;
    }

    public X509Certificate2 Create(string identifier, string certName, string password)
    {
        // Create a new RSA provider for the client certificate
        // using (var rsaProvider = RSA.Create(4096))
        X509Certificate2 caCert = _caManager.GetFromPfx();
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
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                true // Add NonRepudiation if needed
            );

            request.CertificateExtensions.Add(keyUsage);

            var serialNumber = GenerateSerialNumber();
            // BigInteger a = new BigInteger(serialNumber);

            // Use the CA certificate to sign the client certificate request
            // var caCert = new X509Certificate2("ca.pfx", "password");
            var certificate = request.Create(caCert, DateTimeOffset.Now.AddMinutes(-1), DateTimeOffset.Now.AddYears(1),
                serialNumber);

            certificate = certificate.CopyWithPrivateKey(rsaProvider);

            return certificate;
        }
    }

    public string GetRSAPublicKeyFromCertificate(X509Certificate2 certificate)
    {
        // Sertifikadan RSA public key'i al
        using (var rsa = certificate.GetRSAPublicKey())
        {
            var parameters = rsa!.ExportParameters(false);

            // Public key'i ASN.1 / DER formatında export et
            byte[] publicKeyBytes;
            using (var rsaNew = RSA.Create())
            {
                rsaNew.ImportParameters(parameters);
                publicKeyBytes = rsaNew.ExportSubjectPublicKeyInfo();
            }

            // Public key'i Base64 formatında string olarak döndür
            return Convert.ToBase64String(publicKeyBytes);
        }
    }

    public string EncryptDataWithPublicKey(string publicPhase, string payloadData)
    {
        using (RSA rsa = RSA.Create(4096))
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(payloadData);
            rsa.ImportParameters(ConvertFromPem(publicPhase));
            return Convert.ToBase64String(rsa.Encrypt(dataBytes, RSAEncryptionPadding.Pkcs1));
        }
    }
    /// <summary>
    /// Added to simulate client decryption
    /// </summary>
    /// <param name="encryptedData"></param>
    /// <param name="privateKeyPem"></param>
    /// <returns></returns>
    public string DecryptDataWithPrivateKey(byte[] encryptedData, string privateKeyPem)
    {
        using (RSA rsa = RSA.Create(4096))
        {
            rsa.ImportFromPem(privateKeyPem.ToCharArray());

            byte[] decryptedBytes = rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
    /// <summary>
    /// Added to simulate client sign
    /// </summary>
    /// <param name="data"></param>
    /// <param name="privateKeyPem"></param>
    /// <returns></returns>
    public string SignDataWithRSA(string data, string privateKeyPem)
    {
        using (RSA rsa = RSA.Create())
        {
            rsa.ImportFromPem(privateKeyPem.ToCharArray());

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256.HashData(dataBytes);

            var signBytes= rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signBytes);
        }
    }

    public bool Verify(string dataToVerify, string signedData, string publicPhase)
    {
        using (RSA rsa = RSA.Create(4096))
        {
            byte[] dataToVerifyBytes = System.Text.Encoding.UTF8.GetBytes(dataToVerify);
            byte[] signedDataBytes = Convert.FromBase64String(signedData);
            
            rsa.ImportParameters(ConvertFromPem(publicPhase));
            return rsa.VerifyData(dataToVerifyBytes, signedDataBytes, HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }
    }
    
    //Public key'i RSAParameters formatına çevirme
    static RSAParameters ConvertFromPem(string pem)
    {
        // PEM formatını temizle
        // string header = "-----BEGIN CERTIFICATE-----";
        // string footer = "-----END CERTIFICATE-----";
        string header = "-----BEGIN PUBLIC KEY-----";
        string footer = "-----END PUBLIC KEY-----";
        string base64 = pem
            .Replace(header, "")
            .Replace(footer, "")
            .Replace("\n", "")
            .Replace("\r", "");

        // Base64 string'i byte dizisine çevir
        byte[] keyBytes = Convert.FromBase64String(base64);
        
        using (var rsa = RSA.Create(4096))
        {
            rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
            return rsa.ExportParameters(false);
        }

        #region certificate parsing and export public key.

        // Sertifikayı oluştur
        // var certificate = new X509Certificate2(keyBytes);
        //
        // // Sertifikadan RSA public key'i al
        // using (var rsa = certificate.GetRSAPublicKey())
        // {
        //     return rsa.ExportParameters(false);
        // }

        #endregion
    }

    static RSAParameters GetRSAParametersFromKey(string keyValue)
    {
        byte[] modulusBytes = System.Text.Encoding.UTF8.GetBytes(keyValue);
        Array.Resize(ref modulusBytes, 256);
        byte[] exponentBytes = { 1, 0, 1 };

        RSAParameters rsaParameters = new RSAParameters
        {
            Modulus = modulusBytes,
            Exponent = exponentBytes
        };

        return rsaParameters;
    }
    
    // PEM formatındaki özel anahtarı RSAParameters formatına çevirme
    static RSAParameters ConvertPemToRsaParameters(string privateKeyString)
    {
        // PEM formatını temizle
        string header = "-----BEGIN RSA PRIVATE KEY-----";
        string footer = "-----END RSA PRIVATE KEY-----";
        string base64 = privateKeyString.Replace(header, "").Replace(footer, "").Replace("\n", "").Replace("\r", "");

        // Base64 string'i byte dizisine çevir
        byte[] keyBytes = Convert.FromBase64String(base64);

        using (var rsa = RSA.Create(4096))
        {
            rsa.ImportRSAPrivateKey(keyBytes, out _);
            return rsa.ExportParameters(true);
        }
    }   

    public string Decrypt(string privatePhase, string payloadData)
    {
        using (RSA rsa = RSA.Create(4096))
        {
            byte[] dataBytes = Convert.FromBase64String(payloadData);
            rsa.ImportParameters(ConvertPemToRsaParameters(privatePhase));
            
            return Convert.ToBase64String(rsa.Decrypt(dataBytes, RSAEncryptionPadding.Pkcs1));
        }
    }

    public string Signed(string privatePhase, string payloadData)
    {
        using (RSA rsa = RSA.Create(4096))
        {
            byte[] dataBytes = Convert.FromBase64String(payloadData);
            rsa.ImportParameters(ConvertPemToRsaParameters(privatePhase));
            return Convert.ToBase64String(rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
        }
    }

    // Generate a new serial number for a certificate
    byte[] GenerateSerialNumber()
    {
        // Create a new RNGCryptoServiceProvider to generate random bytes
        using (var rng = RandomNumberGenerator.Create())
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
