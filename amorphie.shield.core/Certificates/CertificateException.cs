namespace amorphie.shield.Certificates;

public class NonActiveCertificateException : BusinessException
{
    public NonActiveCertificateException()
    {
    }
}

public class ExpiredCertificateException : BusinessException
{
    public ExpiredCertificateException()
    {
    }
}
