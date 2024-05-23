using System.Security.Cryptography.X509Certificates;

namespace amorphie.shield.CertManager;
public class FileCaManager : ICaManager
{
    private readonly string password = "password";
    private readonly string caPfxFileName = Path.Combine("Certficate", "ca.pfx");


    public FileCaManager() { }
    public async ValueTask<X509Certificate2> GetFromPfxAsync()
    {
        var caCert = await Task.Run(()=> new X509Certificate2(caPfxFileName, password));
        return caCert;
    }
}

