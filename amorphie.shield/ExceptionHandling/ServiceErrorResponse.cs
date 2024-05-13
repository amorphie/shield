namespace amorphie.shield.ExceptionHandling
{
    [Serializable]
    public class ServiceErrorResponse
    {
        public ServiceErrorInfo Error { get; set; }

        public ServiceErrorResponse(ServiceErrorInfo error)
        {
            Error = error;
        }
    }

    [Serializable]
    public class ServiceErrorInfo
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; }
        public string? Severity { get; set; }
    }
}