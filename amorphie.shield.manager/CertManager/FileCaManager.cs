using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace amorphie.shield.CertManager;
public class FileCaManager :ICaManager
{
    private readonly string password = "password";
    private readonly string caPfxFileName = Path.Combine("Certficate", "ca.pfx");


    public FileCaManager() { }
    public X509Certificate2 GetFromPfx()
    {
        var caCert = new X509Certificate2(caPfxFileName, password);
        return caCert;
    }

}

