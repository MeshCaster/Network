namespace Core.Application.DTOs.Domain;

public class RouteDto
{
    public Guid Id { get; set; }
    public Guid SourceNodeId { get; set; }
    public Guid DestinationNodeId { get; set; }
    public List<Guid> PathNodeIds { get; set; } = new();
    public List<string> PathNodeNames { get; set; } = new();
    public int HopCount { get; set; }
    public int TotalLatency { get; set; }
    public long MinThroughput { get; set; }
    public double AverageQuality { get; set; }
    public string Preference { get; set; } = string.Empty;
    public DateTime CalculatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsValid { get; set; }
}