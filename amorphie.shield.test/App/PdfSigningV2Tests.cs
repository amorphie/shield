﻿
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using iText.Bouncycastle.Crypto;
using iText.Bouncycastle.X509;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
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
using Microsoft.AspNetCore.Components.Forms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;




namespace amorphie.shield.test;
public class PdfSigningV2Tests
{
    private readonly string password = "password";


    [Fact]
    public async Task Pdf_Modify_TestAsync()
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
    }

    [Fact]
    public async Task Pdf_Sign_TestAsync()
    {
        var sourcePdf = Path.Combine("Certficate", "dummy.pdf");
        var output = Path.Combine("Certficate", "dummy_sign.pdf");

        var pfxKeyPath = Path.Combine("Certficate", "ca.pfx");
        X509Certificate2 cert = new X509Certificate2(pfxKeyPath, password, X509KeyStorageFlags.Exportable);


        var privateKeyPath = Path.Combine("Certficate", "ca.private.key");
        var privateKeyString = File.ReadAllText(privateKeyPath);
        var privateKey = RSA.Create();
        privateKey.ImportFromPem(privateKeyString.ToCharArray());

        //Gets private key from pfx
        //var privateKey2 = cert.GetRSAPrivateKey();
        //var exported = privateKey2.ExportEncryptedPkcs8PrivateKey("temp", new PbeParameters(PbeEncryptionAlgorithm.TripleDes3KeyPkcs12, HashAlgorithmName.SHA1, 1));
        //RSA privateKey = RSA.Create();
        //privateKey.ImportEncryptedPkcs8PrivateKey("temp", exported, out _);



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
            signer.SetLocation("Burgan Bank Ankara");
            signer.SetPageRect(new iText.Kernel.Geom.Rectangle(100, 800, 300, 100));
            var ssa = new iText.Forms.Form.Element.SignatureFieldAppearance("IdOfAppearance");
            ssa.SetSignerName("ibrahim karakayalı");
            ssa.SetPageNumber(1);

            ssa.SetBold();
            ssa.SetItalic();
            ssa.SetUnderline();
            ssa.SetContent("Test Description for content" + DateTime.Now);
            ssa.SetDestination("A Destination");
            ssa.SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA));
            ssa.SetFontSize(14);
            ssa.SetHeight(14);

            signer.SetSignatureAppearance(ssa);
            //var tsaUrl = "https://freetsa.org/tsr";
            //// Timestamp the signature
            //TSAClientBouncyCastle tsaClient = new TSAClientBouncyCastle(tsaUrl);
            //signer.Timestamp(tsaClient, null);



            //PdfSignatureAppearance sap = signer.GetSignatureAppearance();
            //sap.SetLayer2Text(string.Empty);
            //sap.SetPageNumber(1);
            //sap.SetLayer2Text(DateTime.Now.ToString());
            //sap.SetRenderingMode(PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION);
            //sap.SetLayer2FontSize(7);
            //sap.SetPageRect(new iText.Kernel.Geom.Rectangle(50, 700, 200, 100));
            //PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            //sap.SetLayer2Font(font);

            signer.SignDetached(privateSignature, signChain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
        }


        // Assert
        Assert.NotNull(cert);
        Assert.IsType<X509Certificate2>(cert);
    }


    [Fact]
    public async Task Pdf_Verify_TestAsync()
    {
        var signed = Path.Combine("Certficate", "dummy_signed.pdf");
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

                //var publicKeyPath = Path.Combine("Certficate", "ca.public.key");
                //var publicKeyString = File.ReadAllText(publicKeyPath);
                //var publicKey = RSA.Create(4096);

                //byte[] keyBytes = Convert.FromBase64String(publicKeyString);

                //publicKey.ImportRSAPublicKey(keyBytes, out _);
                //var publicRsaKey = DotNetUtilities.GetRsaPublicKey(publicKey);

                SignatureUtil signatureUtil = new SignatureUtil(pdfDoc);

                foreach (string signatureName in signatureUtil.GetSignatureNames())
                {
                    var signer = signatureUtil.GetTranslatedFieldName(signatureName);
                    var aa = signatureUtil.GetSignature(signatureName).GetCert();
                    var aab = signatureUtil.GetSignature(signatureName).GetReason();
                    var signerName = signatureUtil.GetSignature(signatureName).GetName();

                    var content = signatureUtil.GetSignature(signatureName).GetContents();
                    var scontent = content.ToUnicodeString();
                    PdfPKCS7 pkcs7 = signatureUtil.ReadSignatureData(signatureName);


                    var signName = pkcs7.GetSignName();
                    var timestamp =pkcs7.GetTimeStampDate();
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

                        }
                    }
                }
            }
        }
    }

    [Fact]
    public async Task Pdf_Verify_TestAsync2()
    {
        var signed = Path.Combine("Certficate", "dummy_sign.pdf");
        using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(signed)))
        {
            SignatureUtil signUtil = new SignatureUtil(pdfDoc);
            IList<string> signatureNames = signUtil.GetSignatureNames();

            foreach (var sigName in signatureNames)
            {
                Console.WriteLine($"Signature name: {sigName}");
                PdfPKCS7 pkcs7 = signUtil.ReadSignatureData(sigName);
                bool isSignatureValid = pkcs7.VerifySignatureIntegrityAndAuthenticity();
                Console.WriteLine($"Signature valid: {isSignatureValid}");

                var signingCert = pkcs7.GetSigningCertificate();
                Console.WriteLine($"Signed by: {signingCert.GetSubjectDN()}");

                // Print out certificate details
                var certs = pkcs7.GetCertificates();
                foreach (var cert in certs)
                {
                    Console.WriteLine($"Issuer: {cert.GetIssuerDN()}");
                    //Console.WriteLine($"Subject: {cert.SubjectDN}");
                    //Console.WriteLine($"Valid from: {cert.NotBefore}");
                    //Console.WriteLine($"Valid until: {cert.NotAfter}");
                }

                Console.WriteLine($"Document modified: {!isSignatureValid}");
            }
        }
    }


    public PdfPKCS7 VerifySignature(SignatureUtil signUtil, String name)
    {
        PdfPKCS7 pkcs7 = signUtil.ReadSignatureData(name);

        //CertificateVerification.VerifyCertificates(certs, ks, cal);
        Console.Out.WriteLine("Signature covers whole document: " + signUtil.SignatureCoversWholeDocument(name));
        Console.Out.WriteLine("Document revision: " + signUtil.GetRevision(name) + " of "
                              + signUtil.GetTotalRevisions());
        Console.Out.WriteLine("Integrity check OK? " + pkcs7.VerifySignatureIntegrityAndAuthenticity());
        return pkcs7;
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






