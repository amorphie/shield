namespace amorphie.shield.manager.Shared
{
    public class IdentityDto
    {
        public Guid DeviceId { get; set; }
        public Guid TokenId { get; set; }
        public Guid RequestId { get; set; }
        public int UserTCKN { get; set; }
    }
}