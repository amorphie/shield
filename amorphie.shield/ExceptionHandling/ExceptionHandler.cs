using amorphie.shield.Transactions;
using Microsoft.AspNetCore.Diagnostics;

namespace amorphie.shield.ExceptionHandling
{
    public class ExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is TransactionSignedException)
            {
                var transactionException = exception as TransactionSignedException;
                httpContext.Response.StatusCode = transactionException!.Code;
                await httpContext.Response.WriteAsJsonAsync(
                    new ServiceErrorResponse(
                        new ServiceErrorInfo()
                        {
                            Code = transactionException.Code,
                            Severity = transactionException.Severity,
                            Message = transactionException.Message,
                            Details = transactionException.Details
                        }
                    )
                );
            } 
            else if (exception is BusinessException)
            {
                var transactionException = exception as BusinessException;
                httpContext.Response.StatusCode = transactionException!.Code;
                await httpContext.Response.WriteAsJsonAsync(
                    new ServiceErrorResponse(
                        new ServiceErrorInfo()
                        {
                            Code = transactionException.Code,
                            Severity = transactionException.Severity,
                            Message = transactionException.Message,
                            Details = transactionException.Details
                        }
                    )
                );
            }
            else
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(
                    new ServiceErrorResponse(
                        new ServiceErrorInfo()
                        {
                            Message = exception.Message
                        }
                    )
                );
            }
            return true;
        }
    }
}
