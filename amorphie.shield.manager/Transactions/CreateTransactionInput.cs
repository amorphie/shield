using amorphie.shield.Shared;

namespace amorphie.shield.Transactions;

public class CreateTransactionInput
{
    public required IdentityDto Identity { get; set; }
    public Guid? InstanceId { get; set; }
    public Dictionary<string, object>? Data { get; set; }
}

public class CreateTransactionOutput
{
    public CreateTransactionOutput(
        Guid transactionId,
        string encrptData
    )
    {
        TransactionId = transactionId;
        EncrptData = encrptData;
    }

    public Guid TransactionId { get; set; }

    /// <summary>
    /// Raw data (with nonce) ecnr.
    /// </summary>
    public string EncrptData { get; set; }
}

public class VerifyTransactionInput
{
    public required IdentityDto Identity { get; set; }

    /// <summary>
    /// Raw data (with nonce)
    /// </summary>
    public required Dictionary<string, object> RawData { get; set; }

    public required string SignData { get; set; }
}

public class VerifyTransactionOutput
{
    public VerifyTransactionOutput(bool verified)
    {
        Verified = verified;
    }

    public bool Verified { get; set; }
}
