using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Domain.Interfaces;

public interface INodeRepository : IRepository<Node>
{
    Task<IEnumerable<Node>> GetByTypeAsync(NodeType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Node>> GetByStatusAsync(NodeStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Node>> GetOnlineNodesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Node>> GetByOwnerAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Node>> GetWithinRadiusAsync(double latitude, double longitude, double radiusMeters, CancellationToken cancellationToken = default);
    Task<Node?> GetByMacAddressAsync(string macAddress, CancellationToken cancellationToken = default);
    Task<IEnumerable<Node>> GetNeighborsAsync(Guid nodeId, CancellationToken cancellationToken = default);
    Task<int> GetOnlineCountAsync(CancellationToken cancellationToken = default);
}
