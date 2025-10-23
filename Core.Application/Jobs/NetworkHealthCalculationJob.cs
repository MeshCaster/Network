using AutoMapper;
using Core.Application.DTOs.Domain;
using Core.Application.Hubs;
using Core.Application.Services;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Core.Application.Jobs;

/// <summary>
/// Background job to calculate and broadcast network health
/// </summary>
public class NetworkHealthCalculationJob
{
    private readonly NetworkHealthService _healthService;
    private readonly NetworkHubService _hubService;
    private readonly IMapper _mapper;
    private readonly ILogger<NetworkHealthCalculationJob> _logger;
    
    public NetworkHealthCalculationJob(
        NetworkHealthService healthService,
        NetworkHubService hubService,
        IMapper mapper,
        ILogger<NetworkHealthCalculationJob> logger)
    {
        _healthService = healthService;
        _hubService = hubService;
        _mapper = mapper;
        _logger = logger;
    }
    
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Starting network health calculation job");
        
        try
        {
            var health = await _healthService.CalculateNetworkHealthAsync();
            var healthDto = _mapper.Map<NetworkHealthDto>(health);
            
            healthDto.HealthStatus = health.OverallScore switch
            {
                >= 80 => "Excellent",
                >= 60 => "Good",
                >= 40 => "Fair",
                >= 20 => "Poor",
                _ => "Critical"
            };
            
            // Broadcast to all connected clients
            await _hubService.BroadcastNetworkHealthAsync(healthDto);
            
            _logger.LogInformation("Network health calculated: {Score}", health.OverallScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in network health calculation job");
            throw;
        }
    }
}