namespace Core.Application.DTOs.Domain;

public class NodeConnectionDto
{
    public Guid Id { get; set; }
    public Guid SourceNodeId { get; set; }
    public string SourceNodeName { get; set; } = string.Empty;
    public Guid TargetNodeId { get; set; }
    public string TargetNodeName { get; set; } = string.Empty;
    public double Quality { get; set; }
    public int Latency { get; set; }
    public long Throughput { get; set; }
    public double PacketLoss { get; set; }
    public int Rssi { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime EstablishedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsHealthy { get; set; }
}