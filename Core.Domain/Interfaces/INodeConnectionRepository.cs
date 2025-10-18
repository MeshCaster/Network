using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface INodeConnectionRepository : IRepository<NodeConnection>
{
    Task<IEnumerable<NodeConnection>> GetByNodeAsync(Guid nodeId, CancellationToken cancellationToken = default);
    Task<NodeConnection?> GetConnectionAsync(Guid sourceId, Guid targetId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NodeConnection>> GetHealthyConnectionsAsync(CancellationToken cancellationToken = default);
    Task<int> GetHealthyConnectionsCountAsync(CancellationToken cancellationToken = default);
}