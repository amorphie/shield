using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace amorphie.shield.HealthChecks;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddDapr(
        this IHealthChecksBuilder builder,
        string? name = default,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        return builder.Add(new HealthCheckRegistration(
            name ?? DaprHealthCheck.Name,
            sp => new DaprHealthCheck(sp.GetRequiredService<DaprClient>()),
            failureStatus,
            tags,
            timeout));
    }

    public static IHealthChecksBuilder AddDaprSecretStore(
       this IHealthChecksBuilder builder,
       string? name = default,
       HealthStatus? failureStatus = default,
       IEnumerable<string>? tags = default,
       TimeSpan? timeout = default)
    {
        return builder.Add(new HealthCheckRegistration(
            name ?? DaprSecretStoreHealthCheck.Name,
            sp => new DaprSecretStoreHealthCheck(sp.GetRequiredService<DaprClient>()),
            failureStatus,
            tags,
            timeout));
    }
}