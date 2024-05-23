using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace amorphie.shield.CertManager;
public interface ICaManager
{
    ValueTask<X509Certificate2> GetFromPfxAsync();
}

