namespace Core.Application.DTOs.Domain;

public class NodeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public LocationDto Location { get; set; } = null!;
    public double SignalStrength { get; set; }
    public long BandwidthCapacity { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double Temperature { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastHeartbeat { get; set; }
    public bool IsOnline { get; set; }
    public Guid? OwnerId { get; set; }
    public string? OwnerUsername { get; set; }
}