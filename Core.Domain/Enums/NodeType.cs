namespace Core.Domain.Enums;

/// <summary>
/// Types of nodes in the mesh network
/// </summary>
public enum NodeType
{
    /// <summary>
    /// Gateway node - connects mesh to internet/external networks
    /// </summary>
    Gateway = 1,
    
    /// <summary>
    /// Backbone node - high-capacity relay infrastructure
    /// </summary>
    Backbone = 2,
    
    /// <summary>
    /// Distribution node - mid-tier relay serving local areas
    /// </summary>
    Distribution = 3,
    
    /// <summary>
    /// User device - end-user devices (phones, laptops, IoT)
    /// </summary>
    UserDevice = 4
}