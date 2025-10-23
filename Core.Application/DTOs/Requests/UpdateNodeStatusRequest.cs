namespace Core.Application.DTOs.Requests;

public class UpdateNodeStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public double SignalStrength { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsage { get; set; }
    public double Temperature { get; set; }
}
