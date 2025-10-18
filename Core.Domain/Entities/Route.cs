using Core.Domain.Enums;

namespace Core.Domain.Entities;

/// <summary>
/// Represents a calculated path through the mesh network
/// </summary>
public class Route
{
    public Guid Id { get; set; }
    
    public Guid SourceNodeId { get; set; }
    public Node SourceNode { get; set; } = null!;
    
    public Guid DestinationNodeId { get; set; }
    public Node DestinationNode { get; set; } = null!;
    
    // Path details (stored as JSON array of node IDs)
    public List<Guid> PathNodeIds { get; set; } = new List<Guid>();
    
    // Route metrics
    public int HopCount { get; set; }
    public int TotalLatency { get; set; } // milliseconds
    public long MinThroughput { get; set; } // bits per second (bottleneck)
    public double AverageQuality { get; set; }
    
    // Routing preference used
    public RoutingPreference Preference { get; set; }
    
    // Cache metadata
    public DateTime CalculatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsValid { get; set; }
    
    // Statistics
    public int UsageCount { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
