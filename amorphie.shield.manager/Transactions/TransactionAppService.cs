using System.Text.Json;
using amorphie.core.Base;
using amorphie.shield.Certificates;
using amorphie.shield.CertManager;

namespace amorphie.shield.Transactions;

public class TransactionAppService : ITransactionAppService
{
    private readonly CertificateRepository _certificateRepository;
    private readonly TransactionRepository _transactionRepository;

    public TransactionAppService(
        CertificateRepository certificateRepository,
        TransactionRepository transactionRepository)
    {
        _certificateRepository = certificateRepository;
        _transactionRepository = transactionRepository;
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
            JsonSerializer.Serialize(input.Data),
            TransactionType.Encrypted
        );

        var encryptedData = CertificateUtil.EncryptDataWithPublicKey(certificate.PublicCert, transaction.Data);
        await _transactionRepository.InsertAsync(transaction, cancellationToken);
        return Response<CreateTransactionOutput>.Success(
            "success",
            new CreateTransactionOutput(transaction.Id, input.Data, encryptedData)
        );
    }

    public async Task<Response<VerifyTransactionOutput>> VerifyAsync(VerifyTransactionInput input, CancellationToken cancellationToken = default)
    {
        var certificate = await _certificateRepository.FindClientsPublicKeyByDeviceAndUserActiveAsync(
            input.Identity.DeviceId,
            input.Identity.UserTCKN,
            cancellationToken
            );
        if (input.TransactionId == null)
        {
            return await VerifyNonEncryptedAsync(input, certificate, cancellationToken);
        }
        else
        {
            return await VerifyEncryptedAsync(input, certificate, cancellationToken);
        }
    }

    public async Task<Response<VerifyTransactionOutput>> VerifyNonEncryptedAsync(VerifyTransactionInput input, Certificate certificate, CancellationToken cancellationToken = default)
    {
        var isVerify = CertificateUtil.Verify(
            certificate.PublicCert,
            JsonSerializer.Serialize(input.RawData),
            input.SignData
        );

        if (!isVerify)
        {
            throw new TransactionSignedException();
        }
        var transaction = new Transaction(
            certificate.Id,
            null,
            input.Identity.RequestId,
            JsonSerializer.Serialize(input.RawData),
            TransactionType.NonEncrypted
        );
        transaction.Verified(
            requestId: input.Identity.RequestId,
            payloadData: JsonSerializer.Serialize(input.RawData),
            signSignature: input.SignData
        );
        await _transactionRepository.InsertAsync(transaction, cancellationToken);

        return Response<VerifyTransactionOutput>.Success(
            "success",
            new VerifyTransactionOutput(true)
        );
    }
    public async Task<Response<VerifyTransactionOutput>> VerifyEncryptedAsync(VerifyTransactionInput input, Certificate certificate, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetAsync(input.TransactionId!.Value, cancellationToken);
        var isVerify = CertificateUtil.Verify(
            certificate.PublicCert,
            transaction.Data,
            input.SignData
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

        await _transactionRepository.UpdateAsync(transaction, cancellationToken);

        return Response<VerifyTransactionOutput>.Success(
            "success",
            new VerifyTransactionOutput(true)
        );
    }



}
