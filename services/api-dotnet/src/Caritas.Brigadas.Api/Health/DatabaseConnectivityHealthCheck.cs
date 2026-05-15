using Caritas.Brigadas.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Caritas.Brigadas.Api.Health;

public sealed class DatabaseConnectivityHealthCheck : IHealthCheck
{
    private readonly CaritasDbContext _dbContext;
    private readonly ILogger<DatabaseConnectivityHealthCheck> _logger;

    public DatabaseConnectivityHealthCheck(
        CaritasDbContext dbContext,
        ILogger<DatabaseConnectivityHealthCheck> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

            if (canConnect)
            {
                return HealthCheckResult.Healthy("Database connectivity check passed.");
            }

            return HealthCheckResult.Unhealthy("Database connectivity check failed.");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Database connectivity health check failed.");

            return HealthCheckResult.Unhealthy(
                "Database connectivity check failed.",
                exception);
        }
    }
}
