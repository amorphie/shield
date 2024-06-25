using System.ComponentModel.DataAnnotations.Schema;
using amorphie.core.Base;
using amorphie.shield.Shared;

namespace amorphie.shield.Certificates;

/// <summary>
/// Certificate
/// </summary>
public sealed class Certificate : EntityBase
{
    /// <summary>
    /// Workflow Instance Id
    /// </summary>
    public Guid? InstanceId { get; private set; }
    public Identity Identity { get; private set; }
    /// <summary>
    /// Certificate CN
    /// </summary>
    public string Cn { get; private set; }
    /// <summary>
    /// Serial Number
    /// </summary>
    public string SerialNumber { get; private set; } = default!;
    /// <summary>
    /// Public Cert
    /// </summary>
    public string PublicCert { get; private set; } = default!;
    /// <summary>
    /// Thumb Print
    /// </summary>
    public string? ThumbPrint { get; private set; }
    /// <summary>
    /// Active, Passive, Revoked
    /// </summary>
    public CertificateStatus Status { get; private set; } = default!; 
    
    /// <summary>
    /// Server, Client
    /// </summary>
    public CertificateOrigin Origin { get; private set; } = default!;
    /// <summary>
    /// Status Reason
    /// </summary>
    public string? StatusReason { get; private set; }
    /// <summary>
    /// Revocation Date
    /// </summary>
    public DateTime? RevocationDate { get; private set; }
    /// <summary>
    /// Expiration Date
    /// </summary>
    public DateTime ExpirationDate { get; private set; }

    [NotMapped]
    public bool IsExpiry => DateTime.UtcNow > ExpirationDate;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Certificate()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        //For ORM
    }

    public Certificate(
        string cn,
        string deviceId,
        Guid tokenId,
        Guid requestId,
        string userTckn,
        Guid? instanceId,
        string serialNumber,
        string publicCert,
        string? thumbprint,
        DateTime expirationDate,
        CertificateOrigin origin
    )
    {
        Cn = cn;
        Identity = new Identity(
            deviceId, 
            tokenId, 
            requestId, 
            userTckn);
        InstanceId = instanceId;
        SerialNumber = serialNumber;
        PublicCert = publicCert;
        ThumbPrint = thumbprint;
        ExpirationDate = expirationDate;
        Origin = origin;
    }

    public void Active(){
        Status = CertificateStatus.Active;
        StatusReason = "Actived";
    }

    public void Passive(string? reason){
        Status = CertificateStatus.Passive;
        StatusReason = reason;
    }

    public void Revoked(string? reason){
        Status = CertificateStatus.Revoked;
        RevocationDate = DateTime.UtcNow;
        StatusReason = reason;
    }
}
