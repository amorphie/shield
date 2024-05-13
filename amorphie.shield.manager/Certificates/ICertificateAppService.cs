using amorphie.core.Base;

namespace amorphie.shield.Certificates;

public interface ICertificateAppService
{
    Task<Response<CertificateCreateOutputDto>> CreateAsync(CertificateCreateInputDto input);
    Task<Response<CertificateQueryOutputDto>> GetBySerialNumberAsync(string serialNumber);
    Task<Response<CertificateQueryOutputDto>> GetByDeviceIdAsync(Guid deviceId);
}
