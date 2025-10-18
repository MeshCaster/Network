namespace Core.Domain.Entities;

/// <summary>
/// Records relay contributions for credit calculation
/// </summary>
public class RelayTransaction
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public Guid NodeId { get; set; }
    public Node Node { get; set; } = null!;
    
    // Transaction details
    public long BytesRelayed { get; set; }
    public decimal CreditsEarned { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    // Quality multiplier applied
    public double QualityMultiplier { get; set; }
    public double TimeOfDayMultiplier { get; set; }
    
    // Calculated at
    public DateTime ProcessedAt { get; set; }
}