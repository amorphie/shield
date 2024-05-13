namespace amorphie.shield.Transactions
{
    public interface ITransactionAppService
    {
        Task<CreateTransactionOutput> CreateAsync(CreateTransactionInput input);
        Task<VerifyTransactionOutput> VerifyAsync(Guid transactionId, VerifyTransactionInput input);
    }
}
