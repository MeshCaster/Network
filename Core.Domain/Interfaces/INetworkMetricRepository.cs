using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface INetworkMetricRepository : IRepository<NetworkMetric>
{
    Task<IEnumerable<NetworkMetric>> GetByNodeAsync(Guid nodeId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<NetworkMetric?> GetLatestAsync(Guid nodeId, CancellationToken cancellationToken = default);
    Task DeleteOlderThanAsync(DateTime threshold, CancellationToken cancellationToken = default);
}