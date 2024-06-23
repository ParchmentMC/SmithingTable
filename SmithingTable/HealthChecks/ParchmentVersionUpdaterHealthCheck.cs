using Microsoft.Extensions.Diagnostics.HealthChecks;
using SmithingTable.Services;

namespace SmithingTable.HealthChecks;

public class ParchmentVersionUpdaterHealthCheck : IHealthCheck
{
    private readonly IParchmentVersionUpdater _parchmentVersionUpdater;

    public ParchmentVersionUpdaterHealthCheck(IParchmentVersionUpdater parchmentVersionUpdater)
    {
        _parchmentVersionUpdater = parchmentVersionUpdater;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            if (await _parchmentVersionUpdater.CanUpdateVersionsAsync(cancellationToken))
            {
                return HealthCheckResult.Healthy("Parchment maven is reachable");
            }
            else
            {
                return HealthCheckResult.Degraded("Parchment maven is not reachable");
            }
        } catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Parchment maven is not reachable", ex);
        }
    }
}