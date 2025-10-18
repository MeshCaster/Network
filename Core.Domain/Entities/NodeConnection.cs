using Core.Domain.Enums;

namespace Core.Domain.Entities;

/// <summary>
/// Represents a bidirectional connection between two nodes
/// </summary>
public class NodeConnection
{
    public Guid Id { get; set; }
    
    // Connection endpoints
    public Guid SourceNodeId { get; set; }
    public Node SourceNode { get; set; } = null!;
    
    public Guid TargetNodeId { get; set; }
    public Node TargetNode { get; set; } = null!;
    
    // Connection quality metrics
    public double Quality { get; set; } // 0-100 score
    public int Latency { get; set; } // milliseconds
    public long Throughput { get; set; } // bits per second
    public double PacketLoss { get; set; } // percentage 0-100
    public int Rssi { get; set; } // Received Signal Strength Indicator (dBm)
    
    // Status
    public ConnectionStatus Status { get; set; }
    public DateTime EstablishedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    
    // Statistics
    public long BytesSent { get; set; }
    public long BytesReceived { get; set; }
    
    public bool IsHealthy() => 
        Status == ConnectionStatus.Active && 
        Quality > 40 && 
        PacketLoss < 10;
}