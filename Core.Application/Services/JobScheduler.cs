using Core.Application.Jobs;
using Hangfire;

namespace Core.Application.Services;

/// <summary>
/// Configure and schedule all background jobs
/// </summary>
public static class JobScheduler
{
    public static void ConfigureRecurringJobs()
    {
        // Route refresh - every 5 minutes
        RecurringJob.AddOrUpdate<RouteRefreshJob>(
            "route-refresh",
            job => job.ExecuteAsync(),
            "*/5 * * * *"); // Every 5 minutes

        // Health check - every 1 minute
        RecurringJob.AddOrUpdate<HealthCheckJob>(
            "health-check",
            job => job.ExecuteAsync(),
            "* * * * *"); // Every minute

        // Network health calculation - every 10 minutes
        RecurringJob.AddOrUpdate<NetworkHealthCalculationJob>(
            "network-health-calculation",
            job => job.ExecuteAsync(),
            "*/10 * * * *"); // Every 10 minutes

        // Metrics cleanup - daily at 2 AM
        RecurringJob.AddOrUpdate<MetricsCleanupJob>(
            "metrics-cleanup",
            job => job.ExecuteAsync(),
            "0 2 * * *"); // Daily at 2 AM

        // Relay credit calculation - daily at midnight
        RecurringJob.AddOrUpdate<RelayCreditCalculationJob>(
            "relay-credit-calculation",
            job => job.ExecuteAsync(),
            "0 0 * * *"); // Daily at midnight
    }
}