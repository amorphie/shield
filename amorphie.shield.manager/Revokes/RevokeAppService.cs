using amorphie.core.Base;
using amorphie.shield.Certificates;

namespace amorphie.shield.Revokes;

public class RevokeAppService : IRevokeAppService
{
    private readonly CertificateRepository _certificateRepository;

    public RevokeAppService(
        CertificateRepository certificateRepository)
    {
        _certificateRepository = certificateRepository;
    }

    public async Task<Response<RevokeTokenOutput>> RevokeTokenAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        var certificate = await _certificateRepository.GetByTokenAsync(tokenId, cancellationToken);
        certificate.Revoked("token request");
        await _certificateRepository.UpdateAsync(certificate, cancellationToken);
        return Response<RevokeTokenOutput>.Success("success", new RevokeTokenOutput(true, certificate.RevocationDate!.Value));
    }

    public async Task<Response<RevokeDeviceOutput>> RevokeDeviceAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        var certificate = await _certificateRepository.GetByDeviceAsync(deviceId, cancellationToken);
        certificate.Revoked("device request");
        await _certificateRepository.UpdateAsync(certificate, cancellationToken);
        return Response<RevokeDeviceOutput>.Success("success", new RevokeDeviceOutput(true, certificate.RevocationDate!.Value));
    }

    public async Task<Response<RevokeUserOutput>> RevokeUserAsync(string userTckn, CancellationToken cancellationToken = default)
    {
        var certificate = await _certificateRepository.GetByUserAsync(userTckn, cancellationToken);
        certificate.Revoked("user request");
        await _certificateRepository.UpdateAsync(certificate, cancellationToken);
        return Response<RevokeUserOutput>.Success("success", new RevokeUserOutput(true, certificate.RevocationDate!.Value));
    }

    public async Task<Response<RevokeCertificateOutput>> RevokeCertificateAsync(Guid certificateId, CancellationToken cancellationToken = default)
    {
        var certificate = await _certificateRepository.GetActiveAsync(certificateId, cancellationToken);
        certificate.Revoked("certificate request");
        await _certificateRepository.UpdateAsync(certificate, cancellationToken);
        return Response<RevokeCertificateOutput>.Success("success", new RevokeCertificateOutput(true, certificate.RevocationDate!.Value));
    }
}
