using Core.Domain.Enums;
using NetTopologySuite.Geometries;

namespace Core.Domain.Entities;

/// <summary>
/// Represents a mesh network node (device) in the network topology
/// </summary>
public class Node
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public NodeType Type { get; set; }
    public NodeStatus Status { get; set; }
    
    // Geospatial location
    public Point Location { get; set; } = null!;
    
    // Network properties
    public double SignalStrength { get; set; } // 0-100
    public long BandwidthCapacity { get; set; } // bits per second
    public string IpAddress { get; set; } = string.Empty;
    public string MacAddress { get; set; } = string.Empty;
    
    // Performance metrics
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double Temperature { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime LastHeartbeat { get; set; }
    public DateTime? LastOfflineAt { get; set; }
    
    // Relationships
    public Guid? OwnerId { get; set; }
    public User? Owner { get; set; }
    
    public ICollection<NodeConnection> OutgoingConnections { get; set; } = new List<NodeConnection>();
    public ICollection<NodeConnection> IncomingConnections { get; set; } = new List<NodeConnection>();
    public ICollection<NetworkMetric> Metrics { get; set; } = new List<NetworkMetric>();
    
    // Helper methods
    public bool IsOnline() => 
        Status == NodeStatus.Online && 
        (DateTime.UtcNow - LastHeartbeat).TotalSeconds < 300;
    
    public bool IsHealthy() => 
        IsOnline() && 
        SignalStrength > 30 && 
        CpuUsage < 90 && 
        MemoryUsage < 90;
}