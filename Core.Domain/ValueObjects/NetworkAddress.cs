namespace Core.Domain.ValueObjects;

/// <summary>
/// Value object for network addressing
/// </summary>
public record NetworkAddress
{
    public string IpAddress { get; init; }
    public string MacAddress { get; init; }
    public int? Port { get; init; }
    
    public NetworkAddress(string ipAddress, string macAddress, int? port = null)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address cannot be empty", nameof(ipAddress));
        
        if (string.IsNullOrWhiteSpace(macAddress))
            throw new ArgumentException("MAC address cannot be empty", nameof(macAddress));
        
        if (port.HasValue && (port < 1 || port > 65535))
            throw new ArgumentException("Port must be between 1 and 65535", nameof(port));
        
        IpAddress = ipAddress;
        MacAddress = macAddress;
        Port = port;
    }
}