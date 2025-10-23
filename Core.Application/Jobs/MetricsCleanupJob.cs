using Core.Domain.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Core.Application.Jobs;

/// <summary>
/// Background job to clean up old metrics data
/// </summary>
public class MetricsCleanupJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MetricsCleanupJob> _logger;
    
    public MetricsCleanupJob(IUnitOfWork unitOfWork, ILogger<MetricsCleanupJob> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting metrics cleanup job");
        
        try
        {
            // Delete metrics older than 30 days
            var threshold = DateTime.UtcNow.AddDays(-30);
            await _unitOfWork.Metrics.DeleteOlderThanAsync(threshold);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Metrics cleanup job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in metrics cleanup job");
            throw;
        }
    }
}