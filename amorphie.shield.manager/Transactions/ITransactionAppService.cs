using amorphie.core.Base;

namespace amorphie.shield.Transactions;

public interface ITransactionAppService
{
    Task<Response<CreateTransactionOutput>> CreateAsync(CreateTransactionInput input, CancellationToken cancellationToken = default);
    Task<Response<VerifyTransactionOutput>> VerifyAsync(Guid transactionId, VerifyTransactionInput input, CancellationToken cancellationToken = default);
}
