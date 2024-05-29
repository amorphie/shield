using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using amorphie.shield.Extension;
using System.Formats.Asn1;

namespace amorphie.shield.CertManager;

public class CertificateManager
{
    private readonly ICaManager _caManager;

    public CertificateManager(ICaManager caManager)
    {
        _caManager = caManager;
    }

    public async Task<X509Certificate2> CreateAsync(string identifier, string certName, string password)
    {
        // Create a new RSA provider for the client certificate
        // using (var rsaProvider = RSA.Create(4096))
        X509Certificate2 caCert = await _caManager.GetFromPfxAsync();
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

            request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(false, false, 0, false));

            request.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

            request.CertificateExtensions.Add(
                new X509AuthorityKeyIdentifierExtension());

            request.CertificateExtensions.Add(GenerateOcsp());
            

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


    public static X509Extension GenerateOcsp()
    {
        List<byte[]> encodedUrls = new List<byte[]>();
        List<byte[]> encodedSequences = new List<byte[]>();

        AsnWriter writer = new AsnWriter(AsnEncodingRules.DER);

        writer.WriteObjectIdentifier("1.3.6.1.5.5.7.48.2");
        encodedUrls.Add(writer.Encode());

        writer = new AsnWriter(AsnEncodingRules.DER);
        writer.WriteCharacterString(
            UniversalTagNumber.IA5String,
            "http://ocsp.example.com",
            new Asn1Tag(TagClass.ContextSpecific, 6)
        );

        encodedUrls.Add(writer.Encode());

        writer = new AsnWriter(AsnEncodingRules.DER);
        using (writer.PushSequence())
        {
            foreach (byte[] encodedName in encodedUrls)
            {
                writer.WriteEncodedValue(encodedName);
            }
        }
        encodedSequences.Add(writer.Encode());

        encodedUrls = new List<byte[]>();
        writer = new AsnWriter(AsnEncodingRules.DER);
        writer.WriteObjectIdentifier("1.3.6.1.5.5.7.48.1");
        encodedUrls.Add(writer.Encode());

        writer = new AsnWriter(AsnEncodingRules.DER);
        writer.WriteCharacterString(
            UniversalTagNumber.IA5String,
            "http://ocsp.sectigo.com/",
            new Asn1Tag(TagClass.ContextSpecific, 6)
        );

        encodedUrls.Add(writer.Encode());

        writer = new AsnWriter(AsnEncodingRules.DER);
        using (writer.PushSequence())
        {
            foreach (byte[] encodedName in encodedUrls)
            {
                writer.WriteEncodedValue(encodedName);
            }
        }
        encodedSequences.Add(writer.Encode());



        
        encodedUrls = new List<byte[]>();
        writer = new AsnWriter(AsnEncodingRules.DER);
        writer.WriteObjectIdentifier("2.5.29.31");
        encodedUrls.Add(writer.Encode());

        writer = new AsnWriter(AsnEncodingRules.DER);
        writer.WriteCharacterString(
            UniversalTagNumber.IA5String,
            "http://crl.globalsign.com/gsrsaovsslca2018.crl",
            new Asn1Tag(TagClass.ContextSpecific, 6)
        );

        encodedUrls.Add(writer.Encode());

        writer = new AsnWriter(AsnEncodingRules.DER);
        using (writer.PushSequence())
        {
            foreach (byte[] encodedName in encodedUrls)
            {
                writer.WriteEncodedValue(encodedName);
            }
        }
        encodedSequences.Add(writer.Encode());






        writer = new AsnWriter(AsnEncodingRules.DER);
        using (writer.PushSequence())
        {
            foreach (byte[] encodedSequence in encodedSequences)
            {
                writer.WriteEncodedValue(encodedSequence);
            }
        }

        return new X509Extension(
            new Oid("1.3.6.1.5.5.7.1.1"),
            writer.Encode(),
            false);
    }
    /// <summary>
    /// certificate.GetRSAPrivateKey().ExportRSAPrivateKeyPem() test thisstatement then replace
    /// </summary>
    /// <param name="certificate"></param>
    /// <returns></returns>
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

    public static string EncryptDataWithPublicKey(string publicPhase, string payloadData)
    {
        using (RSA rsa = CreateRSAFromPem(publicPhase))
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(payloadData);
            return Convert.ToBase64String(rsa.Encrypt(dataBytes, RSAEncryptionPadding.Pkcs1));
        }
    }
    /// <summary>
    /// Added to simulate client decryption
    /// </summary>
    /// <param name="encryptedData"></param>
    /// <param name="privateKeyPem"></param>
    /// <returns></returns>
    public static string DecryptDataWithPrivateKey(byte[] encryptedData, string privateKeyPem)
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
    public static string SignDataWithRSA(string data, string privateKeyPem)
    {
        using (RSA rsa = RSA.Create())
        {
            rsa.ImportFromPem(privateKeyPem.ToCharArray());

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256.HashData(dataBytes);

            var signBytes = rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signBytes);
        }
    }

    public static bool Verify(string dataToVerify, string signedData, string publicPhase)
    {
        using (RSA rsa = CreateRSAFromPem(publicPhase))
        {
            byte[] dataToVerifyBytes = System.Text.Encoding.UTF8.GetBytes(dataToVerify);
            byte[] signedDataBytes = Convert.FromBase64String(signedData);

            return rsa.VerifyData(dataToVerifyBytes, signedDataBytes, HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }
    }

    /// <summary>
    /// Pem to RSA
    /// </summary>
    /// <param name="pem"></param>
    /// <returns></returns>
    static RSA CreateRSAFromPem(string pem)
    {
        // PEM formatını temizle
        // string header = "-----BEGIN CERTIFICATE-----";
        // string footer = "-----END CERTIFICATE-----";
        string header = "-----BEGIN PUBLIC KEY-----";
        string header2 = "-----BEGIN RSA PUBLIC KEY-----";
        string footer = "-----END PUBLIC KEY-----";
        string footer2 = "-----END RSA PUBLIC KEY-----";
        string base64 = pem
            .Replace(header, "")
            .Replace(footer, "") 
            .Replace(header2, "")
            .Replace(footer2, "")
            .Replace("\n", "")
            .Replace("\r", "");

        // Base64 string'i byte dizisine çevir
        byte[] keyBytes = Convert.FromBase64String(base64);

        var rsa = RSA.Create(4096);
        rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
        return rsa;
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
