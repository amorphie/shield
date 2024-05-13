namespace amorphie.shield.Transactions
{
    public class TransactionSignedException : BusinessException
    {
        public TransactionSignedException() : base(422, "error", "Data could not be verified.")
        {
        }
    }

    public class TransactionWaitingException : BusinessException
    {
        public TransactionWaitingException() : base(418, "error", "You can only verify transactions with \"Waiting\" status.")
        {
        }
    }

    public class TransactionRejectedException : BusinessException
    {
        public TransactionRejectedException() : base(418, "error", "")
        {
        }
    }
}
