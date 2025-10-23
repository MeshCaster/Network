using AutoMapper;
using Core.Application.Commands.Handler;
using Core.Application.DTOs.Domain;
using Core.Application.Services;

namespace Core.Application.Queries.Handlers;

public class GetNetworkHealthQueryHandler : IRequestHandler<GetNetworkHealthQuery, NetworkHealthDto?>
{
    private readonly NetworkHealthService _healthService;
    private readonly IMapper _mapper;
    
    public GetNetworkHealthQueryHandler(NetworkHealthService healthService, IMapper mapper)
    {
        _healthService = healthService;
        _mapper = mapper;
    }
    
    public async Task<NetworkHealthDto?> Handle(GetNetworkHealthQuery request, CancellationToken cancellationToken)
    {
        var health = await _healthService.GetLatestHealthAsync(cancellationToken);
        if (health == null)
            return null;
        
        var dto = _mapper.Map<NetworkHealthDto>(health);
        dto.HealthStatus = health.OverallScore switch
        {
            >= 80 => "Excellent",
            >= 60 => "Good",
            >= 40 => "Fair",
            >= 20 => "Poor",
            _ => "Critical"
        };
        
        return dto;
    }
}