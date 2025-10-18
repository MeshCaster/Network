using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface INetworkHealthRepository : IRepository<NetworkHealth>
{
    Task<NetworkHealth?> GetLatestAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<NetworkHealth>> GetHistoryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}