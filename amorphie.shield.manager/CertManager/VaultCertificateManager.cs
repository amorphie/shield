using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using VaultSharp.V1.SecretsEngines.PKI;
using VaultSharp;

namespace amorphie.shield.CertManager;
public class VaultCertificateManager : ICertificateManager
{
    private readonly IVaultClient _vaultClient;
    private readonly VaultOptions _vaultOptions;
    public VaultCertificateManager(IVaultClient vaultClient, VaultOptions options)
    {
        _vaultClient = vaultClient;
        _vaultOptions = options;
    }
    public async Task<X509Certificate2> CreateAsync(string identifier)
    {

        ////To tidy up store
        //var request = new CertificateTidyRequest { TidyCertStore = true, TidyRevokedCerts = true };
        //await vaultClient.V1.Secrets.PKI.TidyAsync(request);

        // Generate a private key and CSR using .NET libraries
        using (RSA rsa = RSA.Create(2048))
        {
            var cnInSubject = $"{identifier}.{_vaultOptions.CommonName}";
            var csr = CreateCertificateSigningRequest(rsa, cnInSubject);

            // Convert private key to PEM format
            //var rsaPrivateKeyPem = rsa.ExportRSAPrivateKeyPem();


            // Submit CSR to Vault for signing
            SignCertificatesRequestOptions signCertData = new SignCertificatesRequestOptions() { Csr = csr, CommonName = _vaultOptions.CommonName, TimeToLive = _vaultOptions.TimeToLive };

            var signedCertResponse = await _vaultClient.V1.Secrets.PKI.SignCertificateAsync(_vaultOptions.RoleName, signCertData, _vaultOptions.PkiBackendMountPoint);
            //Create X509
            var certificate = new X509Certificate2(Encoding.UTF8.GetBytes(signedCertResponse.Data.CertificateContent));

            certificate = certificate.CopyWithPrivateKey(rsa);

            return certificate;
        }
    }

    static string CreateCertificateSigningRequest(RSA rsa, string subjectName)
    {
        var request = new CertificateRequest($"CN={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return request.CreateSigningRequestPem();
    }

}

