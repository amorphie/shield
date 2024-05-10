using amorphie.shield.core.Constant;
using amorphie.shield.core.Dto.Certificate;
using amorphie.shield.core.Extension;
using amorphie.shield.core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace amorphie.shield.core.Mapper
{
    public static class CertificateMapper
    {
        public static Certificate Map(CertificateCreateRequestDto certificateCreateRequestDto, X509Certificate2 x509Cert)
        {
            return new Certificate
            {
                InstanceId = certificateCreateRequestDto.InstanceId,
                XDeviceId = certificateCreateRequestDto.XDeviceId,
                XTokenId = certificateCreateRequestDto.XTokenId,
                XRequestId = certificateCreateRequestDto.XRequestId,
                UserTCKN = certificateCreateRequestDto.UserTCKN,
                Cn = x509Cert.CN(),
                SerialNumber = x509Cert.SerialNumber().ToString(),
                PublicCert = x509Cert.ExportCer(),
                ThumbPrint = x509Cert.Thumbprint,
                Status = CertificateStatus.Active,
                StatusReason = "New Record",
                ExpirationDate = x509Cert.NotAfter.ToUniversalTime(),
            };
        }
        public static CertificateCreateResponseDto Map(Certificate certificate, X509Certificate2 x509Cert)
        {
            return new CertificateCreateResponseDto
            {
                Id = certificate.Id,
                Certificate = x509Cert.ExportCer(),
                PrivateKey = x509Cert.GetRSAPrivateKey()?.ExportPrivateKey(),
                ExpirationDate = x509Cert.NotAfter
            };
        }
    }
}
