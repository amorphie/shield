//using System.Security.Cryptography;
//using System.Security.Cryptography.X509Certificates;
//using amorphie.shield.CertManager;
//using iText.IO.Font.Constants;
//using iText.Kernel.Font;
//using iText.Kernel.Pdf;
//using iText.Signatures;


//namespace amorphie.shield.test;
//public class PdfSigningTests
//{
//    private readonly string password = "password";

//    [Fact]
//    public async Task Pdf_Sign_TestAsync()
//    {
//        ICaManager caManager = new FileCaManager();
//        var certManager = new CertificateManager(caManager);
//        var cert = await certManager.CreateAsync("", "testCert", password);
//        var sourcePdf = Path.Combine("Certficate", "dummy.pdf");
//        var signedPdfPath = Path.Combine("Certficate", "signed.pdf");
//        // Extract the private key
//        var privateKey = cert.GetRSAPrivateKey();

//        // Convert the certificate to a form usable by iTextSharp
//        var chain = new Org.BouncyCastle.X509.X509Certificate[] {
//            Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert)
//        };

//        using (FileStream pdfReadStream = new FileStream(sourcePdf, FileMode.Open, FileAccess.Read))
//        using (FileStream pdfWriteStream = new FileStream(signedPdfPath, FileMode.Create, FileAccess.Write))
//        {
//            PdfReader pdfReader = new PdfReader(pdfReadStream);
//            PdfSigner pdfSigner = new PdfSigner(pdfReader, pdfWriteStream, new StampingProperties());

//            PdfSignatureAppearance sap = pdfSigner.GetSignatureAppearance();

//            sap.SetCertificate(Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert));

//            sap.SetLayer2Text(string.Empty);
//            sap.SetPageNumber(1);
//            sap.SetLayer2Text("Date : " + DateTime.Now.ToString());
//            sap.SetRenderingMode(PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION);
//            sap.SetLayer2FontSize(12);
//            sap.SetPageRect(new iText.Kernel.Geom.Rectangle(50, 700, 200, 100));
//            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
//            sap.SetLayer2Font(font);

//            // Sign with the extracted private key
//            IExternalSignature pks = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);

//            // If you have a TSA URL, you can use it to timestamp the signature
//            ITSAClient tsaClient = null;
//            pdfSigner.SignDetached(pks, chain, null, null, tsaClient, 0, PdfSigner.CryptoStandard.CMS);
//        }


//        // Assert
//        Assert.NotNull(cert);
//        Assert.IsType<X509Certificate2>(cert);
//    }
//    [Fact]
//    public async Task Pdf_Verify_TestAsync()
//    {
//        var signed = Path.Combine("Certficate", "signed.pdf");
//        using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(signed)))
//        {
//            SignatureUtil signUtil = new SignatureUtil(pdfDoc);
//            IList<string> signatureNames = signUtil.GetSignatureNames();

//            foreach (var sigName in signatureNames)
//            {
//                Console.WriteLine($"Signature name: {sigName}");
//                PdfPKCS7 pkcs7 = signUtil.ReadSignatureData(sigName);
//                bool isSignatureValid = pkcs7.VerifySignatureIntegrityAndAuthenticity();
//                Console.WriteLine($"Signature valid: {isSignatureValid}");

//                Org.BouncyCastle.X509.X509Certificate signingCert = pkcs7.GetSigningCertificate();
//                Console.WriteLine($"Signed by: {signingCert.SubjectDN}");

//                // Print out certificate details
//                Org.BouncyCastle.X509.X509Certificate[] certs = pkcs7.GetCertificates();
//                foreach (var cert in certs)
//                {
//                    Console.WriteLine($"Issuer: {cert.IssuerDN}");
//                    Console.WriteLine($"Subject: {cert.SubjectDN}");
//                    Console.WriteLine($"Valid from: {cert.NotBefore}");
//                    Console.WriteLine($"Valid until: {cert.NotAfter}");
//                }

//                Console.WriteLine($"Document modified: {!isSignatureValid}");
//            }
//        }
//    }
//}

//public class PrivateKeySignature : IExternalSignature
//{
//    private RSA rsa;
//    private string hashAlgorithm;

//    public PrivateKeySignature(RSA rsa, string hashAlgorithm)
//    {
//        this.rsa = rsa;
//        this.hashAlgorithm = DigestAlgorithms.GetDigest(hashAlgorithm);
//    }

//    public string GetHashAlgorithm()
//    {
//        return hashAlgorithm;
//    }

//    public string GetEncryptionAlgorithm()
//    {
//        return "RSA";
//    }

//    public byte[] Sign(byte[] message)
//    {
//        using (var hasher = HashAlgorithm.Create(hashAlgorithm))
//        {
//            var hash = hasher.ComputeHash(message);
//            return rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
//        }
//    }
//}






