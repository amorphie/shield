using amorphie.shield.CertManager;
using amorphie.shield.Transactions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

            var privateKey = input.GetProperty("privateKey").ToString();
            byte[] privateKeyData = Convert.FromBase64String(privateKey);
            return CertificateUtil.DecryptDataWithPrivateKey(System.Text.Encoding.UTF8.GetString(privateKeyData), encBytes);
        }
        ).WithOpenApi(operation =>
        {
            operation.Summary = "Test purpose!!! Decrypts the input with given private key. ";
            operation.Description="expected body : {\"encryptedData\" :\"encrypted\", \"privateKey\":\"Base64formatted private key\"}";
            return operation;
        });

        
        group.MapPost("/sign", (
                [FromServices] ITransactionAppService transactionAppService,
                [FromBody] dynamic input,
        CancellationToken cancellationToken
            ) =>
        {
            var signData = JsonSerializer.Serialize(input.GetProperty("signData"));
            var privateKey = input.GetProperty("privateKey").ToString();
            byte[] privateKeyData = Convert.FromBase64String(privateKey);

            return CertificateUtil.SignDataWithRSA(System.Text.Encoding.UTF8.GetString(privateKeyData), signData);
        }
        ).WithOpenApi(operation =>
        {
            operation.Summary = "Test purpose!!! Signs the input with given private key. ";
            operation.Description="expected body : {\"signData\" :\"toBeSignedData in json format\", \"privateKey\":\"Base64formatted private key\"}";

            return operation;
        });


        group.MapPost("/verify", async (
                [FromServices] ITransactionAppService transactionAppService,
                [FromBody] VerifyTransactionInput input,
                CancellationToken cancellationToken
            ) => await transactionAppService.VerifyAsync(input, cancellationToken)
        ).WithOpenApi(operation =>
        {
            operation.Summary = "Verify the signed input";
            return operation;
        });


    }
}
