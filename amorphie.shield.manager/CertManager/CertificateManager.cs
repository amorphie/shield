using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using amorphie.shield.Extension;

namespace amorphie.shield.CertManager;

public class CertificateManager : ICertificateManager
{
    private readonly ICaManager _caManager;

    public CertificateManager(ICaManager caManager)
    {
        _caManager = caManager;
    }

    public async Task<X509Certificate2> CreateAsync(string identifier)
    {
        X509Certificate2 caCert = await _caManager.GetFromPfxAsync();
        string certCN = $"{caCert.CN()}";
        if (!string.IsNullOrEmpty(identifier))
            certCN = $"{identifier}.{certCN}";

        using (var rsaProvider = RSA.Create(2048))
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

            request.CertificateExtensions.Add(CertificateUtil.GenerateOcsp());
            
            var serialNumber = CertificateUtil.GenerateSerialNumber();

            // Use the CA certificate to sign the client certificate request
            var certificate = request.Create(caCert, DateTimeOffset.Now.AddMinutes(-1), DateTimeOffset.Now.AddYears(1),
                serialNumber);

            certificate = certificate.CopyWithPrivateKey(rsaProvider);

            return certificate;
        }
    }
}
