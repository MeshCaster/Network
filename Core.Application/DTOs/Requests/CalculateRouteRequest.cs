namespace Core.Application.DTOs.Requests;

public class CalculateRouteRequest
{
    public Guid SourceNodeId { get; set; }
    public Guid DestinationNodeId { get; set; }
    public string Preference { get; set; } = "MinimumLatency";
    public int MaxHops { get; set; } = 5;
}
