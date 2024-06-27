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
        Dictionary<string, object> rawData,
        string encryptData
    )
    {
        TransactionId = transactionId;
        EncryptData = encryptData;
        RawData = rawData;
    }

    public Guid TransactionId { get; set; }

    /// <summary>
    /// Raw data (with nonce) Encrypt
    /// </summary>
    public string EncryptData { get; set; }

    public Dictionary<string, object> RawData { get; set; }
}

public class VerifyTransactionInput
{
    public Guid? TransactionId { get; set; }

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
