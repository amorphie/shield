namespace amorphie.shield.Revokes;

public class RevokeBaseOutput(bool revoked, DateTime revocationDate)
{
    public bool Revoked { get; set; } = revoked;
    public DateTime RevocationDate { get; set; } = revocationDate;
}

public class RevokeTokenOutput(bool revoked, DateTime revocationDate) : RevokeBaseOutput(revoked, revocationDate);

public class RevokeDeviceOutput(bool revoked, DateTime revocationDate) : RevokeBaseOutput(revoked, revocationDate);

public class RevokeUserOutput(bool revoked, DateTime revocationDate) : RevokeBaseOutput(revoked, revocationDate);

public class RevokeCertificateOutput(bool revoked, DateTime revocationDate) : RevokeBaseOutput(revoked, revocationDate);
