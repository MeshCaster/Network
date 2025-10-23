namespace Core.Application.DTOs.Domain;

public class NetworkMetricDto
{
    public Guid Id { get; set; }
    public Guid NodeId { get; set; }
    public DateTime Timestamp { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public double Temperature { get; set; }
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
    public int ActiveConnections { get; set; }
    public double AverageLatency { get; set; }
    public double SignalStrength { get; set; }
}
