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
            // Create a new certificate request for the CA certificate
            var request = new CertificateRequest(new X500DistinguishedName($"CN={cn}, OU=Digital Banking, O=Burgan Bank Turkey, C=TR"), rsaProvider, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Set the Basic Constraints extension to indicate that this is a CA certificate
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(true, true, 0, true));

            // Self-sign the certificate request
            var certificate = request.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10));


            // Export the client certificate to a PFX file
            var clientCertBytes = certificate.Export(X509ContentType.Cert, password);

            return certificate;
        }
    }

    public X509Certificate2 GetFromPfx()
    {
        var caCert = new X509Certificate2(caPfxFileName, password);
        return caCert;
    }

    public X509Certificate2 TryConcretePfxFromKeys()
    {
        var abyPublicKey = File.ReadAllBytes("ca.cer");
        var abyPrivateKey = File.ReadAllText("ca.private_raw.key");
        var certificate = new X509Certificate2(abyPublicKey, string.Empty,
            X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

        var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(abyPrivateKey), out _);

        certificate.PrivateKey = rsa;
        return certificate;
    }


    private void readprivatekey()
    {
        // Create a FileStream for the file
        using (FileStream fileStream = new FileStream("ca.private_raw.key", FileMode.Open, FileAccess.Read))
        {
            // Create a BinaryReader using the FileStream
            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                // Now you can use the BinaryReader to read binary data from the file
                try
                {
                    // Example: Read an integer from the file
                    int intValue = reader.ReadInt32();
                    Console.WriteLine($"Read integer value: {intValue}");

                    // Example: Read a string from the file
                    string stringValue = reader.ReadString();
                    Console.WriteLine($"Read string value: {stringValue}");
                }
                catch (EndOfStreamException)
                {
                    Console.WriteLine("End of file reached.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error reading from file: {ex.Message}");
                }
            }
        }
    }
}

