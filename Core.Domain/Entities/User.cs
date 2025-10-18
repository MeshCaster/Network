using Core.Domain.Enums;

namespace Core.Domain.Entities;

/// <summary>
/// Represents a user in the mesh network system
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    // Relay participation
    public bool IsRelayEnabled { get; set; }
    public decimal RelayCredits { get; set; }
    public long TotalBytesRelayed { get; set; }
    
    // Profile
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    
    // Status
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // Relationships
    public ICollection<Node> Nodes { get; set; } = new List<Node>();
    public ICollection<RelayTransaction> RelayTransactions { get; set; } = new List<RelayTransaction>();
}