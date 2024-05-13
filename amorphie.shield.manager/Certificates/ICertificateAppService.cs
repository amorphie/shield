using amorphie.core.Base;

namespace amorphie.shield.Certificates;

public interface ICertificateAppService
{
    Task<Response<CertificateCreateResponseDto>> CreateAsync(CertificateCreateRequestDto input);
}
