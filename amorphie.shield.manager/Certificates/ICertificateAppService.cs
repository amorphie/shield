using amorphie.core.Base;

namespace amorphie.shield.Certificates;

public interface ICertificateAppService
{
    Task<Response<CertificateCreateOutputDto>> CreateAsync(CertificateCreateInputDto input);
    Task<Response<CertificateQueryOutputDto>> GetBySerialAsync(string serialNumber);
    Task<Response<CertificateQueryOutputDto>> GetBySerialAndUserTcknAsync(string serialNumber, string userTckn);
    Task<Response<CertificateQueryOutputDto>> GetBySerialAndUserTcknAndXTokenIdAsync(string serialNumber, string userTckn, Guid xTokenId);
    Task<Response<CertificateQueryOutputDto>> GetByUserTcknAndXTokenIdAsync(string userTckn, Guid xTokenId);
    Task<Response<CertificateQueryOutputDto>> GetByUserTcknAndXDeviceIdAsync(string userTckn, string xDeviceId);
    Task<Response<CertificateQueryOutputDto>> GetByDeviceIdAsync(string xDeviceId);
}
