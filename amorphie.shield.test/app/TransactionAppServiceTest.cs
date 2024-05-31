using amorphie.shield.Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.test.Helpers;
using amorphie.shield.Transactions;

namespace amorphie.shield.test;

public class TransactionAppServiceTest
{
    private readonly ITransactionAppService _transactionAppService;
    private readonly ICertificateAppService _certificateAppService;
    private readonly CertificateManager _certificateManager;

    public TransactionAppServiceTest(
        ITransactionAppService transactionAppService,
        ICertificateAppService certificateAppService,
        CertificateManager certificateManager)
    {
        _transactionAppService = transactionAppService;
        _certificateAppService = certificateAppService;
        _certificateManager = certificateManager;
    }

    [Fact]
    public async Task Assert_Transaction_Create_Async()
    {
        var certificate = await _certificateAppService.CreateAsync(new CertificateCreateInputDto()
        {
            Identity = StaticData.IdentityDto,
            InstanceId = StaticData.InstanceId,
        });

        Assert.Equal("Success", certificate.Result.Status);

        var response = await _transactionAppService.CreateAsync(new CreateTransactionInput()
        {
            Identity = StaticData.IdentityDto,
            InstanceId = StaticData.InstanceId,
            Data = new Dictionary<string, object>
            {
                { "name", "shield" },
                { "surname", "test" }
            }
        });

        Assert.Equal("Success", response.Result.Status);
    }

    [Fact]
    public async Task Assert_Transaction_Verify_Async()
    {

        var cerRSAPublicKey = CertificateHelper.GetClientPublicKeyFromFile();

        Assert.NotNull(cerRSAPublicKey);

        var cerPrivateKey = CertificateHelper.GetClientPrivateKeyFromFile();
        Assert.NotNull(cerPrivateKey);


        //Create transaction
        var transactionCreateResponse = await _transactionAppService.CreateAsync(new CreateTransactionInput()
        {
            Identity = StaticData.IdentityDto,
            InstanceId = StaticData.InstanceId,
            Data = new Dictionary<string, object>
            {
                { "name", "shield" },
                { "surname", "test" }
            }
        });

        Assert.Equal("Success", transactionCreateResponse.Result.Status);

        //Data decrypt
        var decryptData = _certificateManager.Decrypt(
            cerPrivateKey,
            transactionCreateResponse.Data.EncryptData
        );

        //Data signed
        var signedData = _certificateManager.Signed(
            cerPrivateKey,
            decryptData
        );

        //Data verified
        var response = await _transactionAppService.VerifyAsync(transactionCreateResponse.Data.TransactionId,
            new VerifyTransactionInput()
            {
                Identity = StaticData.IdentityDto,
                RawData = transactionCreateResponse.Data.RawData,
                SignData = signedData
            });

        Assert.Equal("Success", response.Result.Status);
    }
}
