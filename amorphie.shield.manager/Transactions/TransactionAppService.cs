using System.Text.Json;
using amorphie.core.Base;
using amorphie.shield.Certificates;
using amorphie.shield.CertManager;

namespace amorphie.shield.Transactions;

public class TransactionAppService : ITransactionAppService
{
    private readonly CertificateRepository _certificateRepository;
    private readonly TransactionRepository _transactionRepository;
    private readonly CertificateManager _certificateManager;

    public TransactionAppService(
        CertificateRepository certificateRepository,
        TransactionRepository transactionRepository,
        CertificateManager certificateManager)
    {
        _certificateRepository = certificateRepository;
        _transactionRepository = transactionRepository;
        _certificateManager = certificateManager;
    }

    public async Task<Response<CreateTransactionOutput>> CreateAsync(CreateTransactionInput input,
        CancellationToken cancellationToken = default)
    {
        var certificate = await _certificateRepository.FindByDeviceAndUserActiveAsync(input.Identity.DeviceId,
            input.Identity.UserTCKN, cancellationToken);
        input.Data ??= new Dictionary<string, object>();
        input.Data.Add("nonce", Guid.NewGuid().ToString());
        var transaction = new Transaction(
            certificate.Id,
            input.InstanceId,
            input.Identity.RequestId,
            JsonSerializer.Serialize(input.Data)
        );

        var encryptedData = CertificateManager.EncryptDataWithPublicKey(certificate.PublicCert, transaction.Data);
        await _transactionRepository.InsertAsync(transaction, cancellationToken);
        return Response<CreateTransactionOutput>.Success(
            "success",
            new CreateTransactionOutput(transaction.Id, input.Data, encryptedData)
        );
    }

    public async Task<Response<VerifyTransactionOutput>> VerifyAsync(Guid transactionId,
        VerifyTransactionInput input, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetAsync(transactionId, cancellationToken);
        var certificate = await _certificateRepository.FindByDeviceAndUserActiveAsync(
            input.Identity.DeviceId,
            input.Identity.UserTCKN, 
            cancellationToken
            );
        
        var isVerify = CertificateManager.Verify(
            transaction.Data,
            input.SignData,
            certificate.PublicCert
        );

        if (!isVerify)
        {
            throw new TransactionSignedException();
        }

        transaction.Verified(
            requestId: input.Identity.RequestId,
            payloadData: JsonSerializer.Serialize(input.RawData),
            signSignature: input.SignData
        );

        return Response<VerifyTransactionOutput>.Success(
            "success",
            new VerifyTransactionOutput(true)
        );
    }
}
