// MeshNetwork.Infrastructure/Data/Repositories/NodeConnectionRepository.cs
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Core.Infrastructure.Data;

namespace Core.Infrastructure.Repository;

public class NodeConnectionRepository : Repository<NodeConnection>, INodeConnectionRepository
{
    public NodeConnectionRepository(MeshDbContext context) : base(context) { }
    
    public async Task<IEnumerable<NodeConnection>> GetByNodeAsync(Guid nodeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.SourceNodeId == nodeId || c.TargetNodeId == nodeId)
            .Include(c => c.SourceNode)
            .Include(c => c.TargetNode)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<NodeConnection?> GetConnectionAsync(
        Guid sourceId, 
        Guid targetId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(
                c => c.SourceNodeId == sourceId && c.TargetNodeId == targetId,
                cancellationToken);
    }
    
    public async Task<IEnumerable<NodeConnection>> GetHealthyConnectionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Status == ConnectionStatus.Active && 
                        c.Quality > 40 && 
                        c.PacketLoss < 10)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<int> GetHealthyConnectionsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(c => c.Status == ConnectionStatus.Active && 
                             c.Quality > 40 && 
                             c.PacketLoss < 10, 
                cancellationToken);
    }
}