// MeshNetwork.Infrastructure/Data/UnitOfWork.cs
using Microsoft.EntityFrameworkCore.Storage;
using Core.Domain.Interfaces;

namespace Core.Infrastructure.Data;

/// <summary>
/// Unit of Work implementation for managing transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly MeshDbContext _context;
    private IDbContextTransaction? _transaction;
    
    public INodeRepository Nodes { get; }
    public INodeConnectionRepository Connections { get; }
    public IUserRepository Users { get; }
    public IRouteRepository Routes { get; }
    public INetworkMetricRepository Metrics { get; }
    public IRelayTransactionRepository RelayTransactions { get; }
    public INetworkHealthRepository NetworkHealth { get; }
    
    public UnitOfWork(
        MeshDbContext context,
        INodeRepository nodes,
        INodeConnectionRepository connections,
        IUserRepository users,
        IRouteRepository routes,
        INetworkMetricRepository metrics,
        IRelayTransactionRepository relayTransactions,
        INetworkHealthRepository networkHealth)
    {
        _context = context;
        Nodes = nodes;
        Connections = connections;
        Users = users;
        Routes = routes;
        Metrics = metrics;
        RelayTransactions = relayTransactions;
        NetworkHealth = networkHealth;
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }
    
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}