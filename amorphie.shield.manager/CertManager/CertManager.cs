using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace amorphie.shield.app.CertManager;
public class CertManager
{
    public X509Certificate2 Create(X509Certificate2 caCert, string certCN, string certName)
    {
        // Create a new RSA provider for the client certificate
        using (var rsaProvider = RSA.Create(4096))
        {
            // Create a new certificate request for the client certificate
            var request = new CertificateRequest(new X500DistinguishedName($"CN={certCN}, OU=Digital Banking, O=Burgan Bank Turkey, C=TR"), rsaProvider, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            var keyUsage = new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true  // Add NonRepudiation if needed
            );

            request.CertificateExtensions.Add(keyUsage);

            // Use the CA certificate to sign the client certificate request
            var certificate = request.Create(caCert, DateTimeOffset.Now.AddMinutes(-1), DateTimeOffset.Now.AddYears(1), GenerateSerialNumber());

            // Copy private key
            certificate = certificate.CopyWithPrivateKey(rsaProvider);

            // Export the client certificate to a PFX file
            var clientCertBytes = certificate.Export(X509ContentType.Pfx, "password");

            // Save the client certificate to a file
            File.WriteAllBytes($"{certName}.pfx", clientCertBytes);

            // Export the certificate in PEM format
            var certPem = "-----BEGIN CERTIFICATE-----\r\n" +
                          Convert.ToBase64String(certificate.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks) +
                          "\r\n-----END CERTIFICATE-----";

            // Export the private key in PEM format
            var rsaPrivateKey = rsaProvider.ExportRSAPrivateKey();
            var keyPem = "-----BEGIN RSA PRIVATE KEY-----\r\n" +
                         Convert.ToBase64String(rsaPrivateKey, Base64FormattingOptions.InsertLineBreaks) +
                         "\r\n-----END RSA PRIVATE KEY-----";

            // Save the certificate and private key to PEM files
            File.WriteAllText($"{certName}.pem", certPem);
            File.WriteAllText($"{certName}.key", keyPem);

            return certificate;
        }
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

