namespace amorphie.shield.core
{
    public class BusinessException : Exception
    {
        public BusinessException(
            int code = 0, 
            string? severity = "warning", 
            string? message = null, 
            string? details = null,
            Exception innterException = null)
            : base(message, innterException)
        {
            Code = code;
            Severity = severity;
            Details = details;
        }

        public int Code { get; set; }
        public string? Severity { get; set; }
        public string? Details { get; set; }
    }
}