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




namespace amorphie.shield.test;
public class PdfSigningV2Tests
{
    [Fact]
    public void Pdf_Modify_Test()
    {
        var inputFile = Path.Combine("Certficate", "dummy.pdf");
        var outputFile = Path.Combine("Certficate", "dummy_modified.pdf");
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
        var sourcePdf = Path.Combine("Certficate", "dummy.pdf");
        var output = Path.Combine("Certficate", "dummy_sign.pdf");
        var certBasePath = Path.Combine("c:\\cert\\client\\");
        var caBasePath = Path.Combine("c:\\cert\\ca\\ca.cer");

        var pfxKeyPath = Path.Combine(certBasePath, "client.pfx");
        X509Certificate2 cert = new X509Certificate2(pfxKeyPath, StaticData.Password, X509KeyStorageFlags.Exportable);

        var privateKey = CertificateHelper.GetClientPrivateKeyFromFile_RSA();

        //    // Load PDF
        PdfReader pdfReader = new PdfReader(sourcePdf);
        //PdfDocument pdfDocument = new PdfDocument(pdfReader);

        // Load certificate and private key



        // Convert certificate to BouncyCastle format
        Org.BouncyCastle.X509.X509Certificate bcCert = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(cert);
        AsymmetricCipherKeyPair keyPair = DotNetUtilities.GetKeyPair(privateKey);

        // Extract private key object
        RsaPrivateCrtKeyParameters bcPrivateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;

        var pkSig = new PrivateKeySignature(new PrivateKeyBC(bcPrivateKey), DigestAlgorithms.SHA256);

        // Prepare signing elements
        IX509Certificate[] signChain = new IX509Certificate[2];
        signChain[0] = new X509CertificateBC(bcCert);
        signChain[1] = new X509CertificateBC(bcCert);
        IExternalSignature privateSignature = new PrivateKeySignature(new PrivateKeyBC(bcPrivateKey), DigestAlgorithms.SHA256);


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


#pragma warning disable S125 // Sections of code should not be commented out
            //TestOcspClient ocspClient = new TestOcspClient().AddBuilderForCertIssuer(signChain[0], new PrivateKeyBC(bcPrivateKey));
            //signer.SetOcspClient(ocspClient);

            //signer.SignWithBaselineLTAProfile(signerProperties: properties, chain: signChain, externalSignature: pkSig, tsaClient);
            signer.SignWithBaselineLTAProfile(properties, signChain, new PrivateKeyBC(bcPrivateKey), tsaClient);
#pragma warning restore S125 // Sections of code should not be commented out
            //signer.Timestamp(tsaClient);
        }


        // Assert
        Assert.NotNull(cert);
        Assert.IsType<X509Certificate2>(cert);
    }



    [Fact]
    public async Task Pdf_Sign_TestAsync()
    {
        var sourcePdf = Path.Combine("Certficate", "dummy.pdf");
        var output = Path.Combine("Certficate", "dummy_sign.pdf");
        var certBasePath = Path.Combine("c:\\cert\\client\\");

        var pfxKeyPath = Path.Combine(certBasePath, "client.pfx");
        X509Certificate2 cert = new X509Certificate2(pfxKeyPath, StaticData.Password, X509KeyStorageFlags.Exportable);


        var privateKeyPath = Path.Combine(certBasePath, "client.private.key");
        var privateKeyString = File.ReadAllText(privateKeyPath);
        var privateKey = RSA.Create();
        privateKey.ImportFromPem(privateKeyString.ToCharArray());

        //    // Load PDF
        PdfReader pdfReader = new PdfReader(sourcePdf);
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
        var signed = Path.Combine("Certficate", "dummy_sign.pdf");
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




