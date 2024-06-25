

namespace amorphie.shield.test.Helpers;
public static class StaticData
{
    //public static string XDeviceId = Guid.NewGuid().ToString();
    //public static string XTokenId = Guid.NewGuid().ToString();
    //public static string XRequestId = Guid.NewGuid().ToString();

    public static Guid XDeviceId = new Guid("a0000f64-5717-0000-b3fc-2d963f660001");
    public static Guid XTokenId = new Guid("a0000f64-5717-0000-b3fc-2d963f660001");
    public static Guid XRequestId = new Guid("a0000f64-5717-0000-b3fc-2d963f660001");

    public static Guid InstanceId = Guid.NewGuid();

    public static string Token { get; set; } = string.Empty;
}
