using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using amorphie.shield.core.Dto.CaDto;

namespace amorphie.shield.app.CertManager;
public class CAManager
{
    public CAManager() { }
    public CaCreateDto Create()
    {
        using (var rsaProvider = RSA.Create(4096))
        {
            // Create a new certificate request for the CA certificate
            var request = new CertificateRequest(new X500DistinguishedName("CN=ca.burganbank, OU=Digital Banking, O=Burgan Bank Turkey, C=TR"), rsaProvider, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Set the Basic Constraints extension to indicate that this is a CA certificate
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(true, true, 0, true));

            // Self-sign the certificate request
            var certificate = request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(5));

            if (!certificate.HasPrivateKey)
            {
                // Copy private key
                certificate = certificate.CopyWithPrivateKey(rsaProvider);
            }
            // Export the CA certificate to a PFX file
            var caCertBytes = certificate.Export(X509ContentType.Pfx, "password");

            // Save the CA certificate to a file
            File.WriteAllBytes("ca.pfx", caCertBytes);

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
            File.WriteAllText("ca.pem", certPem);
            File.WriteAllText("ca.key", keyPem);
            var caDto = new CaCreateDto
            {
                CaPem = certPem,
                RsaPem = keyPem,
                Cert=certificate,
            };



            return caDto;
        }
    }
}

