namespace Core.Domain.Entities;

/// <summary>
/// Time-series performance data for nodes
/// </summary>
public class NetworkMetric
{
    public Guid Id { get; set; }
    
    public Guid NodeId { get; set; }
    public Node Node { get; set; } = null!;
    
    public DateTime Timestamp { get; set; }
    
    // System metrics
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
    public double Temperature { get; set; }
    
    // Network metrics
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
    public int PacketsSent { get; set; }
    public int PacketsReceived { get; set; }
    public int PacketsDropped { get; set; }
    
    // Connection metrics
    public int ActiveConnections { get; set; }
    public double AverageLatency { get; set; }
    public double SignalStrength { get; set; }
}
