namespace Core.Application.DTOs.Domain;

public class NetworkHealthDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public double OverallScore { get; set; }
    public double NodeAvailabilityScore { get; set; }
    public double ConnectionQualityScore { get; set; }
    public double RouteRedundancyScore { get; set; }
    public double GatewayAvailabilityScore { get; set; }
    public int TotalNodes { get; set; }
    public int OnlineNodes { get; set; }
    public int TotalConnections { get; set; }
    public int HealthyConnections { get; set; }
    public int AvailableGateways { get; set; }
    public int TotalRoutes { get; set; }
    public string HealthStatus { get; set; } = string.Empty;
}
