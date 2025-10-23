namespace Core.Application.DTOs.Requests;

public class CreateConnectionRequest
{
    public Guid SourceNodeId { get; set; }
    public Guid TargetNodeId { get; set; }
    public double Quality { get; set; }
    public int Latency { get; set; }
    public long Throughput { get; set; }
    public double PacketLoss { get; set; }
    public int Rssi { get; set; }
}