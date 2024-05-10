using amorphie.core.Base;

namespace amorphie.shield.core.Model;

public class Transaction : EntityBase
{

    public Guid? InstanceId { get; set; }
    public Guid? XRequestId { get; set; }
    public string Data { get; set; } = "";
    public string? SignSignature { get; set; }

    public Certificate Certificate { get; set; } = default!;
    public Guid CertificateId { get; set; }

    public DateTime? SignedAt { get; set; }
    /// <summary>
    /// waiting, signed, reject
    /// Status = waiting olanlar => signed olacak sadece
    /// </summary>
    public string? Status { get; set; }
    public ICollection<TransactionActivity>? Activities { get; private set; }
}
