using amorphie.core.Base;

namespace amorphie.shield.Revokes;

public interface IRevokeAppService
{
    Task<Response<RevokeTokenOutput>> RevokeTokenAsync(Guid tokenId, CancellationToken cancellationToken = default);
    Task<Response<RevokeDeviceOutput>> RevokeDeviceAsync(Guid deviceId, CancellationToken cancellationToken = default);
    Task<Response<RevokeUserOutput>> RevokeUserAsync(string userTckn, CancellationToken cancellationToken = default);
    Task<Response<RevokeCertificateOutput>> RevokeCertificateAsync(Guid certificateId, CancellationToken cancellationToken = default);
}
