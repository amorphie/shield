namespace amorphie.shield.manager.Shared
{
    public class ResponseBaseInput <T>
    {
        public string Status { get; set; }
        public T Data { get; set; }
    }
}