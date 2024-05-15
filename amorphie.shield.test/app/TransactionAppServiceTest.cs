using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using amorphie.shield.Certificates;
using amorphie.shield.CertManager;
using amorphie.shield.Transactions;
using Microsoft.EntityFrameworkCore;

namespace amorphie.shield.app;

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
        var identity = new Shared.IdentityDto
        {
            DeviceId = AppConsts.DeviceId,
            RequestId = Guid.NewGuid(),
            TokenId = Guid.NewGuid(),
            UserTCKN = AppConsts.UserTckn
        };
        var certificate = await _certificateAppService.CreateAsync(new CertificateCreateInputDto()
        {
            Identity = identity,
            InstanceId = Guid.NewGuid()
        });

        Assert.Equal("Success", certificate.Result.Status);

        var response = await _transactionAppService.CreateAsync(new CreateTransactionInput()
        {
            Identity = identity,
            InstanceId = Guid.NewGuid(),
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
        var identity = new Shared.IdentityDto
        {
            DeviceId = AppConsts.DeviceId,
            RequestId = Guid.NewGuid(),
            TokenId = Guid.NewGuid(),
            UserTCKN = AppConsts.UserTckn
        };

        //Create certificate
        var certificateResponse = await _certificateAppService.CreateAsync(new CertificateCreateInputDto()
        {
            Identity = identity,
            InstanceId = Guid.NewGuid()
        });

        Assert.Equal("Success", certificateResponse.Result.Status);

        //Create transaction
        var transactionCreateResponse = await _transactionAppService.CreateAsync(new CreateTransactionInput()
        {
            Identity = identity,
            InstanceId = Guid.NewGuid(),
            Data = new Dictionary<string, object>
            {
                { "name", "shield" },
                { "surname", "test" }
            }
        });

        Assert.Equal("Success", transactionCreateResponse.Result.Status);
        
        //Data dencrypt
        var dencryptData = _certificateManager.Dencrypt(
           certificateResponse.Data.PrivateKey!,
            transactionCreateResponse.Data.EncrptData
           );

        //Data signed
        var signedData = _certificateManager.Signed(
            certificateResponse.Data.PrivateKey!,
            dencryptData
            );
        
        //Data verified
        var response = await _transactionAppService.VerifyAsync(transactionCreateResponse.Data.TransactionId,
            new VerifyTransactionInput()
            {
                Identity = new Shared.IdentityDto
                {
                    DeviceId = AppConsts.DeviceId,
                    RequestId = Guid.NewGuid(),
                    TokenId = Guid.NewGuid(),
                    UserTCKN = AppConsts.UserTckn
                },
                RawData = transactionCreateResponse.Data.RawData,
                SignData = signedData
            });

        Assert.Equal("Success", response.Result.Status);
    }
}