using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface IRouteRepository : IRepository<Route>
{
    Task<Route?> GetValidRouteAsync(Guid sourceId, Guid destinationId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Route>> GetRoutesBySourceAsync(Guid sourceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Route>> GetExpiredRoutesAsync(CancellationToken cancellationToken = default);
    Task InvalidateRoutesAsync(Guid nodeId, CancellationToken cancellationToken = default);
}
