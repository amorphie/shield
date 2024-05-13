using System.Collections;
using System.Collections.ObjectModel;
using amorphie.core.Base;

namespace amorphie.shield.Transactions;

/// <summary>
/// Transaction Aggregate
/// </summary>
public sealed class Transaction : EntityBase
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Transaction()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        //For ORM
    }

    public Transaction(
        Guid certificateId,
        Guid? instanceId,
        Guid requestId,
        string payloadData
        
    ){
        CertificateId = certificateId;
        InstanceId = instanceId;
        RequestId = requestId;
        Data = payloadData;
        Status = TransactionStatus.Waiting;
        Activities = new Collection<TransactionActivity>();
        AddActivity(requestId, payloadData);
    }

    //TODO: Tayfun: Payload dataya nonce kod eklenecek.

    /// <summary>
    /// Certificate Id
    /// </summary>
    public Guid CertificateId { get; private set; }
    /// <summary>
    /// Instance Id
    /// </summary>
    public Guid? InstanceId { get; private set; }
    /// <summary>
    /// Request Id
    /// </summary>
    public Guid RequestId { get; private set; }
    /// <summary>
    /// Client Payload Data
    /// </summary>
    public string Data { get; private set; }
    /// <summary>
    /// Sign Signature
    /// </summary>
    public string? SignSignature { get; private set; }
    /// <summary>
    /// SignedAt
    /// </summary>
    public DateTime? SignedAt { get; private set; }
    /// <summary>
    /// Status
    /// Waiting, Signed, Rejected
    /// Status = Waiting olanlar => signed olacak sadece
    /// </summary>
    public TransactionStatus Status { get; private set; }
    /// <summary>
    /// Activities
    /// </summary>
    public ICollection<TransactionActivity>? Activities { get; private set; }

    /// <summary>
    /// Transaction signed function
    /// </summary>
    /// <param name="requestId">Request Id</param>
    /// <param name="payloadData">Payload Data</param>
    /// <param name="signSignature">Sign Signature</param>
    /// <exception cref="TransactionSignedException"></exception>
    public void Signed(Guid requestId, string payloadData, string signSignature){
        if(Status != TransactionStatus.Waiting){
            throw new TransactionSignedException();
        }   
        SignedAt = DateTime.UtcNow;
        Status = TransactionStatus.Signed;    
        SignSignature = signSignature;
        AddActivity(requestId, payloadData);
    }

    /// <summary>
    /// Transaction rejected function
    /// </summary>
    /// <param name="requestId">Request Id</param>
    /// <param name="payloadData">Payload Data</param>
    /// <exception cref="TransactionRejectedException"></exception>
    public void Rejected(Guid requestId, string payloadData){
        if(Status != TransactionStatus.Rejected){
            throw new TransactionRejectedException();
        }   
        Status = TransactionStatus.Rejected;    
        AddActivity(requestId, payloadData);
    }

    private void AddActivity(
        Guid requestId,
        string payloadData
    ){
        Activities!.Add(
            new TransactionActivity(
                Id, 
                requestId, 
                payloadData,
                Status
                )
            );
    }
}
