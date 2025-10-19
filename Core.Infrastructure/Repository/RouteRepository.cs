// MeshNetwork.Infrastructure/Data/Repositories/RouteRepository.cs
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Core.Infrastructure.Data;

namespace Core.Infrastructure.Repository;

public class RouteRepository : Repository<Route>, IRouteRepository
{
    public RouteRepository(MeshDbContext context) : base(context) { }
    
    public async Task<Route?> GetValidRouteAsync(
        Guid sourceId, 
        Guid destinationId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.SourceNodeId == sourceId && 
                        r.DestinationNodeId == destinationId &&
                        r.IsValid &&
                        r.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(r => r.CalculatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Route>> GetRoutesBySourceAsync(
        Guid sourceId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.SourceNodeId == sourceId && r.IsValid)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Route>> GetExpiredRoutesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.ExpiresAt <= DateTime.UtcNow || !r.IsValid)
            .ToListAsync(cancellationToken);
    }
    
    public async Task InvalidateRoutesAsync(Guid nodeId, CancellationToken cancellationToken = default)
    {
        var routes = await _dbSet
            .Where(r => r.PathNodeIds.Contains(nodeId))
            .ToListAsync(cancellationToken);
        
        foreach (var route in routes)
        {
            route.IsValid = false;
        }
    }
}