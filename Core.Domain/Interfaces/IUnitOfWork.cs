namespace Core.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern for managing transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    INodeRepository Nodes { get; }
    INodeConnectionRepository Connections { get; }
    IUserRepository Users { get; }
    IRouteRepository Routes { get; }
    INetworkMetricRepository Metrics { get; }
    IRelayTransactionRepository RelayTransactions { get; }
    INetworkHealthRepository NetworkHealth { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}