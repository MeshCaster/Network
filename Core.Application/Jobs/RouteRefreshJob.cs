using Core.Domain.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

/// <summary>
/// Background job to refresh expired routes
/// </summary>
public class RouteRefreshJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RouteRefreshJob> _logger;
    
    public RouteRefreshJob(IUnitOfWork unitOfWork, ILogger<RouteRefreshJob> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting route refresh job");
        
        try
        {
            var expiredRoutes = await _unitOfWork.Routes.GetExpiredRoutesAsync();
            var expiredList = expiredRoutes.ToList();
            
            _logger.LogInformation("Found {Count} expired routes", expiredList.Count);
            
            foreach (var route in expiredList)
            {
                route.IsValid = false;
                await _unitOfWork.Routes.UpdateAsync(route);
            }
            
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Route refresh job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in route refresh job");
            throw;
        }
    }
}