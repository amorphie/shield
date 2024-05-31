

namespace amorphie.shield.test.Helpers;
public static class StaticData
{
    //public static string XDeviceId = Guid.NewGuid().ToString();
    //public static string XTokenId = Guid.NewGuid().ToString();
    //public static string XRequestId = Guid.NewGuid().ToString();
    public static string ClientCertBasePath = "c:\\cert\\client\\";
    public static string CaCertBasePath = "c:\\cert\\ca\\";
    public static string Password = "password";
    public static string UserTCKN = "1234";

    public static Guid XDeviceId = new Guid("a0000f64-5717-0000-b3fc-2d963f660001");
    public static Guid XTokenId = new Guid("a0000f64-5717-0000-b3fc-2d963f660001");
    public static Guid XRequestId = new Guid("a0000f64-5717-0000-b3fc-2d963f660001");

    public static Guid InstanceId = Guid.NewGuid();

    public static string Token { get; set; } = string.Empty;
    public static Shared.IdentityDto IdentityDto
    {
        get
        {
            return new Shared.IdentityDto
            {
                DeviceId = StaticData.XDeviceId.ToString(),
                TokenId = StaticData.XTokenId,
                RequestId = StaticData.XRequestId,
                UserTCKN = "1234"
            };
        }
    }




}
