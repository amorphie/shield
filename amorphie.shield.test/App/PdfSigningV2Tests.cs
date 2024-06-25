using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Tsp;
using iText.Forms;
using iText.Forms.Fields;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Signatures;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using amorphie.shield.test.Helpers;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Exceptions;
using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle.Asn1.X500;
using iText.Commons.Bouncycastle.Asn1;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert.Ocsp;
using iText.Commons.Utils;
using iText.Commons.Bouncycastle.Asn1.Ocsp;
using iText.Commons.Bouncycastle.Asn1.X509;
using iText.Commons.Bouncycastle.Crypto.Generators;
using iText.Commons.Bouncycastle.Math;
using System.Collections;
using amorphie.shield.Extension;

namespace amorphie.shield.test;
public class PdfSigningV2Tests
{
    private readonly string certName = "vaultclient";
    private readonly string  inputFile = Path.Combine("c:\\cert\\pdf", "dummy.pdf");

    [Fact]
    public void Pdf_Modify_Test()
    {
        var outputFile = Path.Combine("c:\\cert\\pdf", "dummy_modified.pdf");
        var textToAdd = "Sample text";
        using var writer = new PdfWriter(outputFile);
        using var outputDocument = new PdfDocument(writer);
        using var reader = new PdfReader(inputFile);
        using var inputDocument = new PdfDocument(reader);
        using var workingDocument = new Document(outputDocument);
        var numberOfPages = inputDocument.GetNumberOfPages();
        for (var i = 1; i <= numberOfPages; i++)
        {
            var page = inputDocument.GetPage(i);
            var newPage = page.CopyTo(outputDocument);
            outputDocument.AddPage(newPage);
            var copyText = new Paragraph(textToAdd);
            workingDocument.ShowTextAligned(
            p: copyText,
                x: UnitConverter.mm2uu(10), y: UnitConverter.mm2uu(10),
                pageNumber: i,
                textAlign: TextAlignment.LEFT, vertAlign: VerticalAlignment.TOP,
                radAngle: 0);
        }
        Assert.True(numberOfPages > 0);
    }

    [Fact]
    public async Task Pdf_PAdes_Sign_TestAsync()
    {
        var output = Path.Combine("c:\\cert\\pdf", "dummy_sign.pdf");
        var certBasePath = Path.Combine("c:\\cert\\client\\");

        var clientPfxPath = Path.Combine(certBasePath, $"{certName}.pfx");
        X509Certificate2 clientCert = new X509Certificate2(clientPfxPath, StaticData.Password, X509KeyStorageFlags.Exportable);
        Org.BouncyCastle.X509.X509Certificate clientBouncyCastleCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(clientCert);
        var itextFormattedClientCert = new X509CertificateBC(clientBouncyCastleCert);


        var clientPrivateKey = CertificateHelper.GetClientPrivateKeyFromFile_RSA(certName);
        AsymmetricCipherKeyPair clientKeyPair = DotNetUtilities.GetKeyPair(clientPrivateKey);
        RsaPrivateCrtKeyParameters bcPrivateKey = (RsaPrivateCrtKeyParameters)clientKeyPair.Private;
        var clientPrivateKeySignature = new PrivateKeySignature(new PrivateKeyBC(bcPrivateKey), DigestAlgorithms.SHA256);

        var caPrivateKey = CertificateHelper.GetVaultCaPrivateKeyFromFile_RSA();
        AsymmetricCipherKeyPair caPrivateKeyPair = DotNetUtilities.GetKeyPair(caPrivateKey);
        var caPrivateKeyBC = new PrivateKeyBC(caPrivateKeyPair.Private);

        var caCertBytes = CertificateHelper.GetVaultCaCertFromFile();
        var caCert = new X509Certificate2(caCertBytes, StaticData.Password);
        Org.BouncyCastle.X509.X509Certificate caCertBC = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(caCert);
        var bcCaCert = new X509CertificateBC(caCertBC);


        //Silinecek
        var tsaCaCertBytes = CertificateHelper.GetTsaCaCertFromFile();
        var tsaCaCert = new X509Certificate2(tsaCaCertBytes, StaticData.Password);
        Org.BouncyCastle.X509.X509Certificate bctsaCaCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(tsaCaCert);

        //Silinecek
        var tsaClientCertBytes = CertificateHelper.GetTsaCertFromFile();
        var tsaClientCert = new X509Certificate2(tsaClientCertBytes, StaticData.Password);
        Org.BouncyCastle.X509.X509Certificate bcTsaClientCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(tsaClientCert);



        //    // Load PDF
        PdfReader pdfReader = new PdfReader(inputFile);
        //PdfDocument pdfDocument = new PdfDocument(pdfReader);

        // Load certificate and private key



        // Convert certificate to BouncyCastle format



        // Extract private key object


        // Prepare signing elements
        IX509Certificate[] signChain = new IX509Certificate[2];
        signChain[0] = itextFormattedClientCert;
        signChain[1] = bcCaCert;
        //signChain[2] = new X509CertificateBC(bctsaCaCert);
        //signChain[3] = new X509CertificateBC(bcTsaClientCert);

        IExternalSignature privateSignature = new PrivateKeySignature(new PrivateKeyBC(bcPrivateKey), DigestAlgorithms.SHA256);


        ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(bcCaCert, caPrivateKeyBC);
        TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(bcCaCert, caPrivateKeyBC);


        // Sign the document
        using (FileStream signedPdf = new FileStream(output, FileMode.Create, FileAccess.ReadWrite))
        {

#pragma warning disable S125 // Sections of code should not be commented out
            //StampingProperties properties = new StampingProperties();
            //properties.UseAppendMode();

#pragma warning restore S125 // Sections of code should not be commented out

            PdfPadesSigner signer = new PdfPadesSigner(pdfReader, signedPdf);
            SignerProperties properties = new SignerProperties();
            properties.SetFieldName("signfield");
            properties.SetSignatureCreator("ibrahim the sign creator");
            properties.SetReason("Formal Sign Enhanced With TSA");
            properties.SetLocation("Burgan Bank Ankara");
            properties.SetPageRect(new iText.Kernel.Geom.Rectangle(100, 500, 300, 100));
            var ssa = new iText.Forms.Form.Element.SignatureFieldAppearance("IdOfAppearance");
            ssa.SetSignerName("ibrahim karakayalı");
            ssa.SetPageNumber(1);
            ssa.SetBold();
            ssa.SetItalic();
            ssa.SetUnderline();
            ssa.SetContent("Test Description for content : " + DateTime.Now);
            ssa.SetDestination("A Destination");
            ssa.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            ssa.SetFontSize(14);
            ssa.SetHeight(14);
            properties.SetSignatureAppearance(ssa);


            var tsaUrl = "https://freetsa.org/tsr";
            // Timestamp the signature
            TSAClientBouncyCastle tsaClient = new TSAClientBouncyCastle(tsaUrl);
            tsaClient.SetTSAInfo(new CustomITSAInfoBouncyCastle());

            //var crlurl = CertificateUtil.GetCRLURL(signChain[0]);
#pragma warning disable S125 // Sections of code should not be commented out
            //TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(signChain[0], new PrivateKeyBC(bcPrivateKey));
            //signer.SetOcspClient(ocspClient);
            //signer.SetCrlClient(new CrlClientOnline("http://localhost:8200/v1/pki/crl"));

            //ICrlClient crlClient = new TestCrlClient().AddBuilderForCertIssuer(caCert, caPrivateKey);
            signer
                .SetOcspClient(ocspClient)
                //.SetCrlClient(crlClient)
                ;

            signer.SignWithBaselineLTAProfile(signerProperties: properties, chain: signChain, externalSignature: clientPrivateKeySignature, tsaClient);
            //signer.SignWithBaselineLTAProfile(properties, signChain, new PrivateKeyBC(bcPrivateKey), tsaClient);
#pragma warning restore S125 // Sections of code should not be commented out
            //signer.Timestamp(tsaClient);
        }


        // Assert
        Assert.NotNull(clientCert);
        Assert.IsType<X509Certificate2>(clientCert);
    }



    [Fact]
    public async Task Pdf_Sign_TestAsync()
    {
        var output = Path.Combine("c:\\cert\\pdf", "dummy_sign.pdf");
        var certBasePath = Path.Combine("c:\\cert\\client\\");

        var pfxKeyPath = Path.Combine(certBasePath, $"{certName}.pfx");
        X509Certificate2 cert = new X509Certificate2(pfxKeyPath, StaticData.Password, X509KeyStorageFlags.Exportable);


        var privateKeyPath = Path.Combine(certBasePath, $"{certName}.private.key");
        var privateKeyString = File.ReadAllText(privateKeyPath);
        var privateKey = RSA.Create();
        privateKey.ImportFromPem(privateKeyString.ToCharArray());

        //    // Load PDF
        PdfReader pdfReader = new PdfReader(inputFile);
        //PdfDocument pdfDocument = new PdfDocument(pdfReader);

        // Load certificate and private key




        // Convert certificate to BouncyCastle format
        Org.BouncyCastle.X509.X509Certificate bcCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert);
        AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetKeyPair(privateKey);

        // Extract private key object
        RsaPrivateCrtKeyParameters bcPrivateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;

        // Prepare signing elements
        IX509Certificate[] signChain = new IX509Certificate[1];
        signChain[0] = new X509CertificateBC(bcCert);
        IExternalSignature privateSignature = new PrivateKeySignature(new PrivateKeyBC(bcPrivateKey), DigestAlgorithms.SHA256);

        // Sign the document
        using (FileStream signedPdf = new FileStream(output, FileMode.Create, FileAccess.ReadWrite))
        {
            StampingProperties properties = new StampingProperties();
            properties.UseAppendMode();

            PdfSigner signer = new PdfSigner(pdfReader, signedPdf, properties);

            //signer.SetFieldName("SignatureName :" + signer.GetNewSigFieldName());
            signer.SetFieldName(signer.GetNewSigFieldName());
            signer.SetSignatureCreator("ibrahim the sign creator");
            signer.SetReason("Formal Sign Enhanced With TSA");
            signer.SetLocation("Burgan Bank Ankara");
            signer.SetPageRect(new iText.Kernel.Geom.Rectangle(100, 500, 300, 100));
            var ssa = new iText.Forms.Form.Element.SignatureFieldAppearance("IdOfAppearance");
            ssa.SetSignerName("ibrahim karakayalı");
            ssa.SetPageNumber(1);

            ssa.SetBold();
            ssa.SetItalic();
            ssa.SetUnderline();
            ssa.SetContent("Test Description for content : " + DateTime.Now);
            ssa.SetDestination("A Destination");
            ssa.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            ssa.SetFontSize(14);
            ssa.SetHeight(14);
            //signer.SetSignDate(DateTime.Now);
            signer.SetSignatureAppearance(ssa);
            var tsaUrl = "https://freetsa.org/tsr";
            // Timestamp the signature
            TSAClientBouncyCastle tsaClient = new TSAClientBouncyCastle(tsaUrl);
            tsaClient.SetTSAInfo(new CustomITSAInfoBouncyCastle());


            signer.SignDetached(privateSignature, signChain, null, null, tsaClient, 0, PdfSigner.CryptoStandard.CMS);
        }


        // Assert
        Assert.NotNull(cert);
        Assert.IsType<X509Certificate2>(cert);
    }

    private class CustomITSAInfoBouncyCastle : ITSAInfoBouncyCastle
    {

        // TimeStampTokenInfo object contains much more information about the timestamp token,
        // like serial number, TST hash algorithm, etc.
        public void InspectTimeStampTokenInfo(ITimeStampTokenInfo info)
        {
            Console.WriteLine(info.GetGenTime());
        }
    }


    [Fact]
    public void Pdf_Verify_Test()
    {
        var signed = Path.Combine("c:\\cert\\pdf", "dummy_sign.pdf");
        using (PdfReader reader = new PdfReader(signed))
        {
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                PdfAcroForm acroForm = PdfAcroForm.GetAcroForm(pdfDoc, false);

                // Get the fields from the AcroForm
                IDictionary<String, PdfFormField> fields = acroForm.GetAllFormFields();
                foreach (var fieldEntry in fields)
                {
                    PdfFormField field = fieldEntry.Value;
                    if (field is PdfSignatureFormField signatureField)
                    {
                        // This is a signature field
                        Console.WriteLine("Found signature field: " + fieldEntry.Key);

                        // Extract the appearance of the signature field
                        PdfWidgetAnnotation widget = signatureField.GetWidgets()[0];
                        PdfDictionary appearanceDictionary = widget.GetAppearanceDictionary();

                        // Optionally, get the normal appearance (N) stream
                        PdfStream normalAppearance = appearanceDictionary.GetAsStream(PdfName.N);

                        if (normalAppearance != null)
                        {
                            Console.WriteLine("Signature appearance found.");
                            // You can further process the normal appearance stream here
                        }

                    }
                }

                SignatureUtil signatureUtil = new SignatureUtil(pdfDoc);

                foreach (string signatureName in signatureUtil.GetSignatureNames())
                {
                    var signer = signatureUtil.GetTranslatedFieldName(signatureName);
                    var reason = signatureUtil.GetSignature(signatureName).GetReason();
                    var signerName = signatureUtil.GetSignature(signatureName).GetName();

                    var content = signatureUtil.GetSignature(signatureName).GetContents();
                    var scontent = content.ToUnicodeString();
                    PdfPKCS7 pkcs7 = signatureUtil.ReadSignatureData(signatureName);

                    //Aux info
                    signatureUtil.SignatureCoversWholeDocument(signatureName);
                    signatureUtil.GetRevision(signatureName);
                    signatureUtil.GetTotalRevisions();
                    pkcs7.VerifySignatureIntegrityAndAuthenticity();


                    var timestamp = pkcs7.GetTimeStampDate();
                    Assert.IsAssignableFrom<DateTime>(timestamp);


                    if (pkcs7.VerifySignatureIntegrityAndAuthenticity())
                    {

                        foreach (IX509Certificate certificate in pkcs7.GetCertificates())
                        {
                            var serial = certificate.GetSerialNumber().ToString();
                            var issuer = certificate.GetIssuerDN().ToString();
                            var subject = certificate.GetSubjectDN().ToString();
                            var endDate = certificate.GetEndDateTime();
                            var notAfter = certificate.GetNotAfter();
                            var notBefore = certificate.GetNotBefore();
                            var clientpublicKey = ((PublicKeyBC)certificate.GetPublicKey()).GetPublicKey();

                            Assert.NotNull(serial);
                            Assert.NotNull(issuer);
                            Assert.NotNull(subject);
                            Assert.NotNull(endDate);
                            Assert.IsAssignableFrom<DateTime>(notAfter);
                            Assert.IsAssignableFrom<DateTime>(notBefore);
                            Assert.NotNull(clientpublicKey);
                        }
                    }
                }
            }
        }
    }

}
public static class UnitConverter
{
    public static float uu2inch(float uu) => uu / 72f;
    public static float inch2uu(float inch) => inch * 72f;

    public static float inch2mm(float inch) => inch * 25.4f;
    public static float mm2inch(float mm) => mm / 25.4f;

    public static float uu2mm(float uu) => inch2mm(uu2inch(uu));
    public static float mm2uu(float mm) => inch2uu(mm2inch(mm));

    public static float degree2radian(float degree) => (float)(degree * Math.PI / 180);
}





public class TestCrlClient : ICrlClient
{
    private readonly IList<TestCrlBuilder> crlBuilders;

    public TestCrlClient()
    {
        crlBuilders = new List<TestCrlBuilder>();
    }

    public virtual TestCrlClient AddBuilderForCertIssuer(TestCrlBuilder crlBuilder
        )
    {
        crlBuilders.Add(crlBuilder);
        return this;
    }

    public virtual TestCrlClient AddBuilderForCertIssuer(IX509Certificate issuerCert
        , IPrivateKey issuerPrivateKey)
    {
        DateTime yesterday = TimeTestUtil.TEST_DATE_TIME.AddDays(-1);
        crlBuilders.Add(new TestCrlBuilder(issuerCert, issuerPrivateKey, yesterday));
        return this;
    }

    public virtual ICollection<byte[]> GetEncoded(IX509Certificate checkCert, String url)
    {
        return crlBuilders.Select((testCrlBuilder) =>
        {
            try
            {
                return testCrlBuilder.MakeCrl();
            }
            catch (Exception ignore)
            {
                throw new PdfException(ignore);
            }
        }
        ).ToList();
    }
}

public class TestCrlBuilder
{
    private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();
    private const String SIGN_ALG = "SHA256withRSA";

    private readonly IPrivateKey issuerPrivateKey;
    private readonly IX509V2CrlGenerator crlBuilder;

    private DateTime nextUpdate = TimeTestUtil.TEST_DATE_TIME.AddDays(30);

    public TestCrlBuilder(IX509Certificate issuerCert, IPrivateKey issuerPrivateKey, DateTime thisUpdate)
    {
        IX500Name issuerCertSubjectDn = issuerCert.GetSubjectDN();
        this.crlBuilder = FACTORY.CreateX509v2CRLBuilder(issuerCertSubjectDn, thisUpdate);
        this.issuerPrivateKey = issuerPrivateKey;
    }

    public TestCrlBuilder(IX509Certificate issuerCert, IPrivateKey issuerPrivateKey)
        : this(issuerCert, issuerPrivateKey, TimeTestUtil.TEST_DATE_TIME.AddDays(-1))
    {
    }

    public virtual void SetNextUpdate(DateTime nextUpdate)
    {
        this.nextUpdate = nextUpdate;
    }

    /// <summary>See CRLReason</summary>
    public virtual void AddCrlEntry(IX509Certificate certificate, DateTime revocationDate, int reason)
    {
        crlBuilder.AddCRLEntry(certificate.GetSerialNumber(), revocationDate, reason);
    }

    public virtual void AddCrlEntry(IX509Certificate certificate, int reason)
    {
        crlBuilder.AddCRLEntry(certificate.GetSerialNumber(), nextUpdate, reason);
    }

    public virtual void AddExtension(IDerObjectIdentifier objectIdentifier, bool isCritical,
        IAsn1Encodable extension)
    {
        crlBuilder.AddExtension(objectIdentifier, isCritical, extension);
    }

    public virtual byte[] MakeCrl()
    {
        crlBuilder.SetNextUpdate(nextUpdate);
        IX509Crl crl = crlBuilder.Build(FACTORY.CreateContentSigner(SIGN_ALG, issuerPrivateKey));
        return crl.GetEncoded();
    }
}

public class TimeTestUtil
{
    private const int MILLIS_IN_DAY = 86_400_000;

    // This method is used to trim the hours of the day, so that two dates could be compared
    // with a day accuracy. We need such a method since in .NET the signing DateTime extracted
    // from the signature depends on the current time zone set on the machine.
    // TODO DEVSIX-5812 Remove the method
    public static long GetFullDaysMillis(double millis)
    {
        return (long)millis / MILLIS_IN_DAY;
    }

    /// <summary>A date time value to be used in test instead of current date time to get consistent results</summary>
    public static DateTime TEST_DATE_TIME = new DateTime(2000, 2, 14, 14, 14, 2, DateTimeKind.Utc);
}

public class TestOcspClient : IOcspClient
{
    private static readonly IBouncyCastleFactory BOUNCY_CASTLE_FACTORY = BouncyCastleFactoryCreator.GetFactory
        ();

    private readonly IDictionary<String, TestOcspResponseBuilder> issuerIdToResponseBuilder = new LinkedDictionary
        <String, TestOcspResponseBuilder>();

    public virtual TestOcspClient AddBuilderForCertIssuer(IX509Certificate cert, IPrivateKey privateKey)
    {
        issuerIdToResponseBuilder.Put(cert.GetSerialNumber().ToString(16), new TestOcspResponseBuilder(cert, privateKey
            ));
        return this;
    }

    public virtual TestOcspClient AddBuilderForCertIssuer(IX509Certificate cert, TestOcspResponseBuilder builder
        )
    {
        issuerIdToResponseBuilder.Put(cert.GetSerialNumber().ToString(16), builder);
        return this;
    }

    public virtual byte[] GetEncoded(IX509Certificate checkCert, IX509Certificate issuerCert, String url)
    {
        byte[] bytes = null;
        try
        {
            ICertID id = SignTestPortUtil.GenerateCertificateId(issuerCert, checkCert.GetSerialNumber(), BOUNCY_CASTLE_FACTORY
                .CreateCertificateID().GetHashSha1());
            TestOcspResponseBuilder builder = issuerIdToResponseBuilder.Get(issuerCert.GetSerialNumber().ToString(16));
            if (builder == null)
            {
                return null;
                throw new ArgumentException("This TestOcspClient instance is not capable of providing OCSP response for the given issuerCert:"
                     + issuerCert.GetSubjectDN().ToString());
            }
            bytes = builder.MakeOcspResponse(SignTestPortUtil.GenerateOcspRequestWithNonce(id).GetEncoded());
        }
        catch (Exception ignored)
        {
            if (ignored is Exception)
            {
                throw (Exception)ignored;
            }
        }
        return bytes;
    }
}

public class TestOcspResponseBuilder
{
    private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();
    private const String SIGN_ALG = "SHA256withRSA";

    private IBasicOcspRespGenerator responseBuilder;

    private IX509Certificate issuerCert;
    private IPrivateKey issuerPrivateKey;

    private ICertStatus certificateStatus;

    private DateTime thisUpdate = TimeTestUtil.TEST_DATE_TIME.AddDays(-1);

    private DateTime nextUpdate = TimeTestUtil.TEST_DATE_TIME.AddDays(30);

    private DateTime producedAt = TimeTestUtil.TEST_DATE_TIME;

    private IX509Certificate[] chain;

    private bool chainSet = false;

    public TestOcspResponseBuilder(IX509Certificate issuerCert, IPrivateKey issuerPrivateKey,
        ICertStatus certificateStatus)
    {
        this.issuerCert = issuerCert;
        this.issuerPrivateKey = issuerPrivateKey;
        this.certificateStatus = certificateStatus;
        IX500Name subjectDN = issuerCert.GetSubjectDN();
        responseBuilder = FACTORY.CreateBasicOCSPRespBuilder(FACTORY.CreateRespID(subjectDN));
    }

    public TestOcspResponseBuilder(IX509Certificate issuerCert, IPrivateKey issuerPrivateKey)
        : this(issuerCert, issuerPrivateKey, FACTORY.CreateCertificateStatus().GetGood())
    {
    }

    public IX509Certificate GetIssuerCert()
    {
        return issuerCert;
    }

    public virtual void SetCertificateStatus(ICertStatus certificateStatus)
    {
        this.certificateStatus = certificateStatus;
    }

    public virtual void SetThisUpdate(DateTime thisUpdate)
    {
        this.thisUpdate = thisUpdate;
    }

    public virtual void SetNextUpdate(DateTime nextUpdate)
    {
        this.nextUpdate = nextUpdate;
    }

    public virtual void SetProducedAt(DateTime producedAt)
    {
        this.producedAt = producedAt;
    }

    public virtual byte[] MakeOcspResponse(byte[] requestBytes)
    {
        IBasicOcspResponse ocspResponse = MakeOcspResponseObject(requestBytes);
        return ocspResponse.GetEncoded();
    }

    public virtual IBasicOcspResponse MakeOcspResponseObject(byte[] requestBytes)
    {
        IOcspRequest ocspRequest = FACTORY.CreateOCSPReq(requestBytes);
        IReq[] requestList = ocspRequest.GetRequestList();

        IX509Extension extNonce = ocspRequest.GetExtension(FACTORY.CreateOCSPObjectIdentifiers()
            .GetIdPkixOcspNonce());
        if (!FACTORY.IsNullExtension(extNonce))
        {
            // TODO ensure
            IX509Extensions responseExtensions = FACTORY.CreateExtensions(new Dictionary<IDerObjectIdentifier, IX509Extension>() {
                {
                    FACTORY.CreateOCSPObjectIdentifiers().GetIdPkixOcspNonce(), extNonce
                }});
            responseBuilder.SetResponseExtensions(responseExtensions);
        }

        foreach (IReq req in requestList)
        {
            responseBuilder.AddResponse(req.GetCertID(), certificateStatus, thisUpdate.ToUniversalTime(), nextUpdate.ToUniversalTime(),
                FACTORY.CreateExtensions());
        }

        if (!chainSet)
        {
            chain = new IX509Certificate[] { issuerCert };
        }
        return responseBuilder.Build(FACTORY.CreateContentSigner(SIGN_ALG, issuerPrivateKey), chain, producedAt);
    }

    public void SetOcspCertsChain(IX509Certificate[] ocspCertsChain)
    {
        chain = ocspCertsChain;
        chainSet = true;
    }
}

public class SignTestPortUtil
{
    private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();

    public static ICertID GenerateCertificateId(IX509Certificate issuerCert, IBigInteger serialNumber, String hashAlgorithm)
    {
        return FACTORY.CreateCertificateID(hashAlgorithm, issuerCert, serialNumber);
    }

    public static IOcspRequest GenerateOcspRequestWithNonce(ICertID id)
    {
        IOcspReqGenerator gen = FACTORY.CreateOCSPReqBuilder();
        gen.AddRequest(id);

        // create details for nonce extension
        IDictionary extensions = new Hashtable();

        extensions[FACTORY.CreateOCSPObjectIdentifiers().GetIdPkixOcspNonce()] = FACTORY.CreateExtension(false,
            FACTORY.CreateDEROctetString(FACTORY.CreateDEROctetString(PdfEncryption.GenerateNewDocumentId()).GetEncoded()));

        gen.SetRequestExtensions(FACTORY.CreateExtensions(extensions));
        return gen.Build();
    }

    //public static IDigest GetMessageDigest(String hashAlgorithm)
    //{
    //    return FACTORY.CreateIDigest(hashAlgorithm);
    //}

    public static IX509Crl ParseCrlFromStream(Stream input)
    {
        return FACTORY.CreateX509Crl(input);
    }

    public static IRsaKeyPairGenerator BuildRSA2048KeyPairGenerator()
    {
        return FACTORY.CreateRsa2048KeyPairGenerator();
    }

    public static T GetFirstElement<T>(ICollection<T> collection)
    {
        return collection.First();
    }
}
