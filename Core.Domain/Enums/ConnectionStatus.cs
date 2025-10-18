namespace Core.Domain.Enums;

/// <summary>
/// Status of connection between nodes
/// </summary>
public enum ConnectionStatus
{
    Active = 1,
    Inactive = 2,
    Degraded = 3,
    Failed = 4
}