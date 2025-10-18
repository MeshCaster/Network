using Core.Domain.Entities;

namespace Core.Domain.Interfaces;

public interface IRelayTransactionRepository : IRepository<RelayTransaction>
{
    Task<IEnumerable<RelayTransaction>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RelayTransaction>> GetUnprocessedAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalCreditsAsync(Guid userId, CancellationToken cancellationToken = default);
}
