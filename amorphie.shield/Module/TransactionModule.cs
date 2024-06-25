using amorphie.shield.CertManager;
using amorphie.shield.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography.Xml;

namespace amorphie.shield.Module;

public static class TransactionModule
{
    public static void RegisterTransactionModuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("transactions");
        group.MapPost("/", async (
                [FromServices] ITransactionAppService transactionAppService,
                [FromBody] CreateTransactionInput input,
                CancellationToken cancellationToken
            ) => await transactionAppService.CreateAsync(input, cancellationToken)
        ).WithOpenApi(operation =>
        {
            operation.Summary = "Creates transaction and appends nonce to input data and encrypts it. ";
            return operation;
        });

        group.MapPost("/decrypt", (
                [FromServices] ITransactionAppService transactionAppService,
                [FromBody] dynamic input,
                CancellationToken cancellationToken
            ) =>
        {
            var encryptData = input.GetProperty("encryptData").ToString();
            var encBytes = Convert.FromBase64String(encryptData);

            var cerPrivateKey = input.GetProperty("privateKey").ToString();
            return CertificateUtil.DecryptDataWithPrivateKey(encBytes, cerPrivateKey);
        }
        ).WithOpenApi(operation =>
        {
            operation.Summary = "Test purpose!!! Decrypts the input with given private key. ";
            return operation;
        });

        
        group.MapPost("/sign", (
                [FromServices] ITransactionAppService transactionAppService,
                [FromBody] dynamic input,
        CancellationToken cancellationToken
            ) =>
        {
            var signData = input.GetProperty("signData").ToString();
            var privateKey = input.GetProperty("privateKey").ToString();
            return CertificateUtil.SignDataWithRSA(privateKey, signData);
        }
        ).WithOpenApi(operation =>
        {
            operation.Summary = "Test purpose!!! Signs the input with given private key. ";
            return operation;
        });


        group.MapPost("/{transactionId}/verify", async (
                [FromServices] ITransactionAppService transactionAppService,
                [FromRoute] Guid transactionId,
                [FromBody] VerifyTransactionInput input,
                CancellationToken cancellationToken
            ) => await transactionAppService.VerifyAsync(transactionId, input, cancellationToken)
        ).WithOpenApi(operation =>
        {
            operation.Summary = "Verify the signed input";
            return operation;
        });


    }
}
