namespace Core.Domain.Entities;

/// <summary>
/// Snapshot of overall network health
/// </summary>
public class NetworkHealth
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    
    // Health components
    public double OverallScore { get; set; } // 0-100
    public double NodeAvailabilityScore { get; set; }
    public double ConnectionQualityScore { get; set; }
    public double RouteRedundancyScore { get; set; }
    public double GatewayAvailabilityScore { get; set; }
    
    // Statistics
    public int TotalNodes { get; set; }
    public int OnlineNodes { get; set; }
    public int TotalConnections { get; set; }
    public int HealthyConnections { get; set; }
    public int AvailableGateways { get; set; }
    public int TotalRoutes { get; set; }
}