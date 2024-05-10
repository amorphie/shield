using System.Security.Cryptography.X509Certificates;

namespace amorphie.shield.app.CertManager;
public class CaProvider
{
    private CaProvider()
    {
    }
    private static readonly Lazy<X509Certificate2> caCert = new(() => (new CAManager()).GetFromPfx());
    public static X509Certificate2 CaCert => caCert.Value;
}

