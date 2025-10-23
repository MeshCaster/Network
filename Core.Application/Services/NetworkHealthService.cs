using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces; 

namespace Core.Application.Services;

/// <summary>
/// Service for calculating network health metrics
/// </summary>
public class NetworkHealthService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public NetworkHealthService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    /// <summary>
    /// Calculate current network health score
    /// </summary>
    public async Task<NetworkHealth> CalculateNetworkHealthAsync(CancellationToken cancellationToken = default)
    {
        var totalNodes = (await _unitOfWork.Nodes.GetAllAsync(cancellationToken)).Count();
        var onlineNodes = await _unitOfWork.Nodes.GetOnlineCountAsync(cancellationToken);
        
        var allConnections = (await _unitOfWork.Connections.GetAllAsync(cancellationToken)).ToList();
        var totalConnections = allConnections.Count;
        var healthyConnections = await _unitOfWork.Connections.GetHealthyConnectionsCountAsync(cancellationToken);
        
        var gatewayNodes = await _unitOfWork.Nodes.GetByTypeAsync(NodeType.Gateway, cancellationToken);
        var availableGateways = gatewayNodes.Count(n => n.IsOnline());
        
        var allRoutes = await _unitOfWork.Routes.GetAllAsync(cancellationToken);
        var validRoutes = allRoutes.Count(r => r.IsValid && r.ExpiresAt > DateTime.UtcNow);
        
        // Calculate component scores
        var nodeAvailabilityScore = totalNodes > 0 
            ? (double)onlineNodes / totalNodes * 100 
            : 0;
        
        var connectionQualityScore = totalConnections > 0
            ? (double)healthyConnections / totalConnections * 100
            : 0;
        
        var routeRedundancyScore = CalculateRouteRedundancy(allConnections, onlineNodes);
        
        var gatewayAvailabilityScore = gatewayNodes.Count() > 0
            ? (double)availableGateways / gatewayNodes.Count() * 100
            : 0;
        
        // Calculate overall score with weights
        var overallScore = 
            (nodeAvailabilityScore * 0.40) +
            (connectionQualityScore * 0.30) +
            (routeRedundancyScore * 0.20) +
            (gatewayAvailabilityScore * 0.10);
        
        var health = new NetworkHealth
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            OverallScore = Math.Round(overallScore, 2),
            NodeAvailabilityScore = Math.Round(nodeAvailabilityScore, 2),
            ConnectionQualityScore = Math.Round(connectionQualityScore, 2),
            RouteRedundancyScore = Math.Round(routeRedundancyScore, 2),
            GatewayAvailabilityScore = Math.Round(gatewayAvailabilityScore, 2),
            TotalNodes = totalNodes,
            OnlineNodes = onlineNodes,
            TotalConnections = totalConnections,
            HealthyConnections = healthyConnections,
            AvailableGateways = availableGateways,
            TotalRoutes = validRoutes
        };
        
        await _unitOfWork.NetworkHealth.AddAsync(health, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return health;
    }
    
    /// <summary>
    /// Calculate route redundancy score (multiple paths availability)
    /// </summary>
    private double CalculateRouteRedundancy(List<NodeConnection> connections, int onlineNodes)
    {
        if (onlineNodes < 2 || connections.Count == 0)
            return 0;
        
        // Simple heuristic: ratio of connections to potential connections
        var maxPossibleConnections = onlineNodes * (onlineNodes - 1) / 2;
        var redundancyRatio = (double)connections.Count / maxPossibleConnections;
        
        // Scale to 0-100 with diminishing returns
        return Math.Min(100, redundancyRatio * 200);
    }
    
    /// <summary>
    /// Get latest network health snapshot
    /// </summary>
    public async Task<NetworkHealth?> GetLatestHealthAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.NetworkHealth.GetLatestAsync(cancellationToken);
    }
}