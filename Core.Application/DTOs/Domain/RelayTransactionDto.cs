namespace Core.Application.DTOs.Domain;

public class RelayTransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid NodeId { get; set; }
    public string NodeName { get; set; } = string.Empty;
    public long BytesRelayed { get; set; }
    public decimal CreditsEarned { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public double QualityMultiplier { get; set; }
    public double TimeOfDayMultiplier { get; set; }
    public DateTime ProcessedAt { get; set; }
}