namespace amorphie.shield.Shared;

public class Identity : ValueObject
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Identity()
    {
        //For ORM
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    internal Identity(
        Guid deviceId,
        Guid tokenId,
        Guid requestId,
        string userTckn
    )
    {
        DeviceId = deviceId;
        TokenId = tokenId;
        RequestId = requestId;
        UserTCKN = userTckn;
    }

    /// <summary>
    /// Request Id
    /// </summary>
    public Guid DeviceId { get; private set; }

    /// <summary>
    /// Token Id
    /// </summary>
    public Guid TokenId { get; private set; }

    /// <summary>
    /// Request Id
    /// </summary>
    public Guid RequestId { get; private set; }

    /// <summary>
    /// User TCKN
    /// </summary>
    public string UserTCKN { get; private set; }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return DeviceId;
        yield return TokenId;
        yield return RequestId;
        yield return UserTCKN;
    }
}
