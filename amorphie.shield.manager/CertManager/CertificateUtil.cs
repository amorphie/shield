using System.Formats.Asn1;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace amorphie.shield.CertManager;
public static class CertificateUtil
{
    private const int RSAKeySizeInBits = 2048;
    public static string EncryptDataWithPublicKey(string publicKeyPem, string payloadData)
    {
        using (RSA rsa = RSA.Create(RSAKeySizeInBits))
        {
            rsa.ImportFromPem(publicKeyPem.ToCharArray());

            byte[] dataBytes = Encoding.UTF8.GetBytes(payloadData);
            return Convert.ToBase64String(rsa.Encrypt(dataBytes, RSAEncryptionPadding.Pkcs1));
        }
    }
    /// <summary>
    /// Added to simulate client decryption
    /// </summary>
    /// <param name="privateKeyPem"></param>
    /// <param name="encryptedData"></param>
    /// <returns></returns>
    public static string DecryptDataWithPrivateKey(string privateKeyPem, byte[] encryptedData)
    {
        using (RSA rsa = RSA.Create(RSAKeySizeInBits))
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
    public static string SignDataWithRSA(string privateKeyPem, string data)
    {
        using (RSA rsa = RSA.Create(RSAKeySizeInBits))
        {
            rsa.ImportFromPem(privateKeyPem.ToCharArray());

            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256.HashData(dataBytes);

            var signBytes = rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signBytes);
        }
    }

    public static bool Verify(string publicKeyPem, string dataToVerify, string signedData)
    {
        using (RSA rsa = RSA.Create(RSAKeySizeInBits))
        {
            rsa.ImportFromPem(publicKeyPem.ToCharArray());

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
    //static RSA CreateRSAFromPem(string pem)
    //{
    //    // PEM formatını temizle
    //    // string header = "-----BEGIN CERTIFICATE-----";
    //    // string footer = "-----END CERTIFICATE-----";
    //    string header = "-----BEGIN PUBLIC KEY-----";
    //    string header2 = "-----BEGIN RSA PUBLIC KEY-----";
    //    string header3 = "-----BEGIN RSA PRIVATE KEY-----";
    //    string footer = "-----END PUBLIC KEY-----";
    //    string footer2 = "-----END RSA PUBLIC KEY-----";
    //    string footer3 = "-----BEGIN RSA PRIVATE KEY-----";
    //    string base64 = pem
    //        .Replace(header, "")
    //        .Replace(footer, "")
    //        .Replace(header2, "")
    //        .Replace(footer2, "")
    //        .Replace(header3, "")
    //        .Replace(footer3, "")
    //        .Replace("\n", "")
    //        .Replace("\r", "");

    //    // Base64 string'i byte dizisine çevir
    //    byte[] keyBytes = Convert.FromBase64String(base64);

    //    var rsa = RSA.Create(4096);
    //    rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
    //    return rsa;
    //}

    // PEM formatındaki özel anahtarı RSAParameters formatına çevirme
    //static RSAParameters ConvertPemToRsaParameters(string privateKeyString)
    //{
    //    // PEM formatını temizle
    //    string header = "-----BEGIN RSA PRIVATE KEY-----";
    //    string footer = "-----END RSA PRIVATE KEY-----";
    //    string base64 = privateKeyString.Replace(header, "").Replace(footer, "").Replace("\n", "").Replace("\r", "");

    //    // Base64 string'i byte dizisine çevir
    //    byte[] keyBytes = Convert.FromBase64String(base64);

    //    using (var rsa = RSA.Create(4096))
    //    {
    //        rsa.ImportRSAPrivateKey(keyBytes, out _);
    //        return rsa.ExportParameters(true);
    //    }
    //}

    //public static string Decrypt(string privatePhase, string payloadData)
    //{
    //    using (RSA rsa = RSA.Create(4096))
    //    {
    //        byte[] dataBytes = Convert.FromBase64String(payloadData);
    //        rsa.ImportParameters(ConvertPemToRsaParameters(privatePhase));

    //        return Convert.ToBase64String(rsa.Decrypt(dataBytes, RSAEncryptionPadding.Pkcs1));
    //    }
    //}

    //public static string Signed(string privatePhase, string payloadData)
    //{
    //    using (RSA rsa = RSA.Create(4096))
    //    {
    //        byte[] dataBytes = Convert.FromBase64String(payloadData);
    //        rsa.ImportParameters(ConvertPemToRsaParameters(privatePhase));
    //        return Convert.ToBase64String(rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
    //    }
    //}

    // Generate a new serial number for a certificate
    public static byte[] GenerateSerialNumber()
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
    public static string GetRSAPublicKeyFromCertificate(X509Certificate2 certificate)
    {
        return certificate.GetRSAPublicKey().ExportSubjectPublicKeyInfoPem();
    }
}
