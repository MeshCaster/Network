// MeshNetwork.Infrastructure/Data/Repositories/NodeRepository.cs
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Core.Infrastructure.Data;
using NetTopologySuite.Geometries;

namespace Core.Infrastructure.Repository;

public class NodeRepository : Repository<Node>, INodeRepository
{
    public NodeRepository(MeshDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Node>> GetByTypeAsync(NodeType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.Type == type)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Node>> GetByStatusAsync(NodeStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.Status == status)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Node>> GetOnlineNodesAsync(CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddSeconds(-300); // 5 minutes
        
        return await _dbSet
            .Where(n => n.Status == NodeStatus.Online && n.LastHeartbeat >= threshold)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Node>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Node>> GetWithinRadiusAsync(
        double latitude, 
        double longitude, 
        double radiusMeters, 
        CancellationToken cancellationToken = default)
    {
        var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var centerPoint = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
        
        return await _dbSet
            .Where(n => n.Location.Distance(centerPoint) <= radiusMeters)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Node?> GetByMacAddressAsync(string macAddress, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(n => n.MacAddress == macAddress, cancellationToken);
    }
    
    public async Task<IEnumerable<Node>> GetNeighborsAsync(Guid nodeId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.OutgoingConnections.Any(c => c.SourceNodeId == nodeId) ||
                       n.IncomingConnections.Any(c => c.TargetNodeId == nodeId))
            .ToListAsync(cancellationToken);
    }
    
    public async Task<int> GetOnlineCountAsync(CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddSeconds(-300);
        
        return await _dbSet
            .CountAsync(n => n.Status == NodeStatus.Online && n.LastHeartbeat >= threshold, 
                       cancellationToken);
    }
    
    public override async Task<Node?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(n => n.Owner)
            .Include(n => n.OutgoingConnections)
            .Include(n => n.IncomingConnections)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }
}
