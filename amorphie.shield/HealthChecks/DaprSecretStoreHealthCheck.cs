using Dapr.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace amorphie.shield.HealthChecks;

public class DaprSecretStoreHealthCheck(DaprClient daprClient) : IHealthCheck
{
    internal const string Name = "shield-secretstore";
    private readonly DaprClient _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var secretName = "shield-secretstore";
            var result = await _daprClient.GetSecretAsync(Name, secretName, cancellationToken: cancellationToken);

            return result != null
                ? HealthCheckResult.Healthy()
                : new HealthCheckResult(context.Registration.FailureStatus);
        }
        catch 
        {
            return new HealthCheckResult(context.Registration.FailureStatus);
        }
    }
}
