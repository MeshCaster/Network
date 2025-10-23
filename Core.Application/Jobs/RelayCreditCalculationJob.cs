using Core.Application.Services;
using Core.Domain.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Core.Application.Jobs;

/// <summary>
/// Background job to calculate daily relay credits for users
/// </summary>
public class RelayCreditCalculationJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly RelayCreditService _creditService;
    private readonly ILogger<RelayCreditCalculationJob> _logger;
    
    public RelayCreditCalculationJob(
        IUnitOfWork unitOfWork,
        RelayCreditService creditService,
        ILogger<RelayCreditCalculationJob> logger)
    {
        _unitOfWork = unitOfWork;
        _creditService = creditService;
        _logger = logger;
    }
    
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting relay credit calculation job");
        
        try
        {
            var relayUsers = await _unitOfWork.Users.GetRelayUsersAsync();
            var count = 0;
            
            foreach (var user in relayUsers)
            {
                // This would normally calculate credits based on actual relay activity
                // For now, this is a placeholder for the daily processing
                _logger.LogDebug("Processing relay credits for user {UserId}", user.Id);
                count++;
            }
            
            _logger.LogInformation("Relay credit calculation completed for {Count} users", count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in relay credit calculation job");
            throw;
        }
    }
}