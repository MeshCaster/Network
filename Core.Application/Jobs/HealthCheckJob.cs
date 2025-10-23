using Core.Application.Hubs;
using Core.Application.Services;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Core.Application.Jobs;

/// <summary>
/// Background job to check node health and mark stale nodes offline
/// </summary>
public class HealthCheckJob
{
    private readonly NodeManagementService _nodeService;
    private readonly NetworkHealthService _healthService;
    private readonly NetworkHubService _hubService;
    private readonly ILogger<HealthCheckJob> _logger;
    
    public HealthCheckJob(
        NodeManagementService nodeService,
        NetworkHealthService healthService,
        NetworkHubService hubService,
        ILogger<HealthCheckJob> logger)
    {
        _nodeService = nodeService;
        _healthService = healthService;
        _hubService = hubService;
        _logger = logger;
    }
    
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting health check job");
        
        try
        {
            // Mark stale nodes as offline
            await _nodeService.MarkStaleNodesOfflineAsync(300);
            
            _logger.LogInformation("Health check job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in health check job");
            throw;
        }
    }
}