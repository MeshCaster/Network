namespace Core.Domain.Enums;

/// <summary>
/// Preference for route calculation
/// </summary>
public enum RoutingPreference
{
    /// <summary>
    /// Minimize total latency
    /// </summary>
    MinimumLatency = 1,
    
    /// <summary>
    /// Maximize throughput (bandwidth)
    /// </summary>
    MaximumThroughput = 2,
    
    /// <summary>
    /// Minimize number of hops
    /// </summary>
    MinimumHops = 3,
    
    /// <summary>
    /// Balance latency and throughput
    /// </summary>
    Balanced = 4
}