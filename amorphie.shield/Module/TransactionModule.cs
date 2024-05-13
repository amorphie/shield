using amorphie.shield.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace amorphie.shield.Module;

public static class TransactionModule
{
    public static void RegisterTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("transactions");
        group.MapPost("/", async (
                [FromServices] ITransactionAppService transactionAppService,
                [FromBody] CreateTransactionInput input,
                CancellationToken cancellationToken
            ) => await transactionAppService.CreateAsync(input, cancellationToken)
        );

        group.MapPost("/{transactionId}/verify", async (
                [FromServices] ITransactionAppService transactionAppService,
                [FromRoute] Guid transactionId,
                [FromBody] VerifyTransactionInput input,
                CancellationToken cancellationToken
            ) => await transactionAppService.VerifyAsync(transactionId, input, cancellationToken)
        );
    }
}
