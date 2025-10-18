using Core.Domain.Enums;

namespace Core.Domain.ValueObjects;

/// <summary>
/// Value object representing connection quality metrics
/// </summary>
public record ConnectionQuality
{
    public double Quality { get; init; } // 0-100
    public int Latency { get; init; } // ms
    public double PacketLoss { get; init; } // percentage
    public int Rssi { get; init; } // dBm
    
    public ConnectionQuality(double quality, int latency, double packetLoss, int rssi)
    {
        if (quality < 0 || quality > 100)
            throw new ArgumentException("Quality must be between 0 and 100", nameof(quality));
        
        if (latency < 0)
            throw new ArgumentException("Latency cannot be negative", nameof(latency));
        
        if (packetLoss < 0 || packetLoss > 100)
            throw new ArgumentException("Packet loss must be between 0 and 100", nameof(packetLoss));
        
        Quality = quality;
        Latency = latency;
        PacketLoss = packetLoss;
        Rssi = rssi;
    }
    
    public bool IsHealthy() => Quality > 40 && PacketLoss < 10 && Latency < 200;
    
    public ConnectionGrade GetGrade()
    {
        return Quality switch
        {
            >= 80 => ConnectionGrade.Excellent,
            >= 60 => ConnectionGrade.Good,
            >= 40 => ConnectionGrade.Fair,
            >= 20 => ConnectionGrade.Poor,
            _ => ConnectionGrade.Critical
        };
    }
}