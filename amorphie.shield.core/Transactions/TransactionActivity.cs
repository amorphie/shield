using amorphie.core.Base;

namespace amorphie.shield.Transactions;

/// <summary>
/// Transaction Activity
/// It keeps a log of every status change that occurs for the transaction.
/// </summary>
public sealed class TransactionActivity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private TransactionActivity()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        //For ORM
    }

    internal TransactionActivity(
        Guid transactionId,
        Guid requestId,
        string payloadData,
        TransactionStatus status
    )
    {
        TransactionId = transactionId;
        RequestId = requestId;
        Data = payloadData;
        Status = status;
    }

    public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

    /// <summary>
    /// <see cref="Transaction"/> Id
    /// </summary>
    public Guid TransactionId { get; private set; }
    /// <summary>
    /// Request Id
    /// </summary>
    public Guid RequestId { get; private set; }
    /// <summary>
    /// Payload Data
    /// </summary>
    public string Data { get; private set; }

    /// <summary>
    /// Status
    /// Waiting, Signed
    /// </summary>
    public TransactionStatus Status { get; private set; }
}
