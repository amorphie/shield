using System.Text.Json;
using amorphie.shield.Certificates;
using amorphie.shield.data;

namespace amorphie.shield.Transactions
{
    public class TransactionAppService : ITransactionAppService
    {
        private readonly ShieldDbContext _dbContext;
        private readonly CertificateRepository _certificateRepository;
        private readonly TransactionRepository _transactionRepository;

        public TransactionAppService(
            ShieldDbContext dbContext,
            CertificateRepository certificateRepository,
            TransactionRepository transactionRepository)
        {
            _dbContext = dbContext;
            _certificateRepository = certificateRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<CreateTransactionOutput> CreateAsync(CreateTransactionInput input)
        {
            var certificate = await _certificateRepository.FindByDeviceActiveAsync(input.Identity.DeviceId);
            CertificateCheck(certificate);

            var transaction = new Transaction(
                certificate.Id,
                input.InstanceId,
                input.Identity.RequestId,
                JsonSerializer.Serialize(input.Data)
                );

            await _transactionRepository.InsertAsync(transaction);
            //TODO: Tayfun: encrpting data
            return CreateTransactionOutput.Success(new CreateTransactionDto(transaction.Id, string.Empty));
        }

        public async Task<VerifyTransactionOutput> VerifyAsync(Guid transactionId, VerifyTransactionInput input)
        {
            var certificate = await _certificateRepository.FindByDeviceActiveAsync(input.Identity.DeviceId);
            CertificateCheck(certificate);

            var transaction = await _transactionRepository.GetAsync(transactionId);

            //TODO: Tayfun: Signing process
            transaction.Signed(input.Identity.RequestId, "", input.SignData);
            return VerifyTransactionOutput.Success(new VerifyTransactionDto(true));
        }

        private void CertificateCheck(Certificate certificate)
        {
            if (certificate == null)
            {
                throw new NonActiveCertificateException();
            }

            if (certificate.IsExpiry)
            {
                throw new ExpiredCertificateException();
            }
        }
    }
}
