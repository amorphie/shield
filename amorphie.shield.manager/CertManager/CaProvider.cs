using System.Security.Cryptography.X509Certificates;

namespace amorphie.shield.app.CertManager;
public class CaProvider
{
    private CaProvider()
    {
    }
    private static readonly Lazy<X509Certificate2> privateCaCert = new(() => CaManager.Instance.GetFromPfx());
    public static X509Certificate2 CaCert => privateCaCert.Value;
}

