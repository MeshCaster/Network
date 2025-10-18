namespace Core.Domain.Enums;

/// <summary>
/// Current operational status of a node
/// </summary>
public enum NodeStatus
{
    Online = 1,
    Offline = 2,
    Degraded = 3,
    Maintenance = 4,
    Error = 5
}
