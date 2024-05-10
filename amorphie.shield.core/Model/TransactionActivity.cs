using amorphie.core.Base;

namespace amorphie.shield.core.Model;

public class TransactionActivity : EntityBase
{
    internal TransactionActivity()
    {
        
    }
    public Guid TransactionId { get; set; }
    public Transaction Transaction { get; set; } = default!;
    public Guid? XRequestId { get; set; }
    public string Data { get; set; } = "";

    /// <summary>
    /// waiting, signed
    /// </summary>
    public string? Status { get; set; }
}
