using amorphie.shield.manager.Shared;

namespace amorphie.shield.Transactions
{
    public class CreateTransactionInput
    {
        public required IdentityDto Identity { get; set; }
        public Guid? InstanceId { get; set; }
        public required dynamic Data { get; set; }
    }

    public class CreateTransactionOutput : ResponseBaseInput<CreateTransactionDto>
    {

        public static CreateTransactionOutput Success(CreateTransactionDto transaction)
        {
            return new CreateTransactionOutput
            {
                Status = "success",
                Data = transaction
            };
        }

        public static CreateTransactionOutput Failed(CreateTransactionDto transaction)
        {
            return new CreateTransactionOutput
            {
                Status = "error",
                Data = transaction
            };
        }
    }

    public class CreateTransactionDto
    {
        public CreateTransactionDto(
            Guid transactionId,
            string encrptData
        )
        {
            TransactionId = transactionId;
            EncrptData = encrptData;
        }

        public Guid TransactionId { get; set; }
        public string EncrptData { get; set; }
    }

    public class VerifyTransactionInput
    {
        public required IdentityDto Identity { get; set; }
        public string SignData { get; set; }
    }

    public class VerifyTransactionOutput : ResponseBaseInput<VerifyTransactionDto>
    {
        public static VerifyTransactionOutput Success(VerifyTransactionDto verifyTransaction)
        {
            return new VerifyTransactionOutput
            {
                Status = "success",
                Data = verifyTransaction
            };
        }

        public static VerifyTransactionOutput Failed(VerifyTransactionDto verifyTransaction)
        {
            return new VerifyTransactionOutput
            {
                Status = "error",
                Data = verifyTransaction
            };
        }
    }

    public class VerifyTransactionDto
    {
        public VerifyTransactionDto(bool verified)
        {
            Verified = verified;
        }
        public bool Verified { get; set; }
    }
}
