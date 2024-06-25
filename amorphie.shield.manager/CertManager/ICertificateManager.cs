using System.Security.Cryptography.X509Certificates;

namespace amorphie.shield.CertManager;
public interface ICertificateManager
{
    Task<X509Certificate2> CreateAsync(string identifier);
}

