// MeshNetwork.Infrastructure/Data/Repositories/Repository.cs

using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Core.Domain.Interfaces;
using Core.Infrastructure.Data;

namespace Core.Infrastructure.Repository;

public class NetworkMetricRepository : Repository<NetworkMetric>, INetworkMetricRepository
{
    public NetworkMetricRepository(MeshDbContext context) : base(context) { }
    
    public async Task<IEnumerable<NetworkMetric>> GetByNodeAsync(
        Guid nodeId, 
        DateTime from, 
        DateTime to, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.NodeId == nodeId && m.Timestamp >= from && m.Timestamp <= to)
            .OrderBy(m => m.Timestamp)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<NetworkMetric?> GetLatestAsync(Guid nodeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.NodeId == nodeId)
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task DeleteOlderThanAsync(DateTime threshold, CancellationToken cancellationToken = default)
    {
        var oldMetrics = await _dbSet
            .Where(m => m.Timestamp < threshold)
            .ToListAsync(cancellationToken);
        
        _dbSet.RemoveRange(oldMetrics);
    }
}