using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace amorphie.shield.CertManager;
public class CaManager
{
    private readonly string password = "password";
    private readonly string caPfxFileName = Path.Combine("Certficate", "ca.pfx");


    public CaManager() { }
    public static CaManager Instance { get; private set; } = new CaManager();
    public X509Certificate2 Create(string cn, string password)
    {
        // using (var rsaProvider = RSA.Create(4096))
        using (var rsaProvider = new RSACryptoServiceProvider(4096))
        {
            /*
            CN (Common Name):           The name of the entity (architect.burganbank.com).
            OU (Organizational Unit):   The department or division within the organization (e.g., "Integration Department").
            O (Organization):           The name of the organization (e.g., "BurganBank").
            L (Locality):               The city or locality (e.g., "İstanbul").
            ST (State or Province):     The state or province (e.g., "İstanbul").
            C (Country):                The two-letter country code (e.g., "TR").
            E (Email Address):          The email address of the entity (e.g., "ali.veli@example.com").
            */
            // Create a new certificate request for the CA certificate
            var request = new CertificateRequest(new X500DistinguishedName($"CN={cn}, OU=Digital Banking, O=Burgan Bank Turkey, C=TR"), rsaProvider, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Set the Basic Constraints extension to indicate that this is a CA certificate
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(true, true, 0, true));

            // Self-sign the certificate request
            var certificate = request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10));

            // Export the client certificate to a PFX file
            var clientCertBytes = certificate.Export(X509ContentType.Pkcs12, password);

            return certificate;
        }
    }

    public X509Certificate2 GetFromPfx()
    {
        var caCert = new X509Certificate2(caPfxFileName, password);
        return caCert;
    }

}

