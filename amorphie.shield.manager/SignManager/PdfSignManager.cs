using System;
using System.ComponentModel.Design;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
namespace amorphie.shield.app.SignManager;
public class PdfSignManager
{
    public PdfDocument? LoadPdf()
    {
        string input = "dummy.pdf";

        try
        {
            // Load PDF
            PdfReader pdfReader = new PdfReader(input);
            return new PdfDocument(pdfReader);


        }
        catch (Exception ex)
        {
            Console.WriteLine("Error signing PDF: " + ex.Message);
            return null;
        }
    }
    public AsymmetricCipherKeyPair? GetCertKeyPair()
    {
        string keystorePath = @"client.pfx";
        string password = "password";

        try
        {
            // Load certificate and private key
            X509Certificate2 cert = new X509Certificate2(keystorePath, password);
            return DotNetUtilities.GetKeyPair(cert.GetRSAPrivateKey());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error signing PDF: " + ex.Message);
            return null;

        }
    }

}

