// MeshNetwork.Infrastructure/Data/Repositories/RelayTransactionRepository.cs
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Core.Infrastructure.Data;

namespace Core.Infrastructure.Repository;

public class RelayTransactionRepository : Repository<RelayTransaction>, IRelayTransactionRepository
{
    public RelayTransactionRepository(MeshDbContext context) : base(context) { }
    
    public async Task<IEnumerable<RelayTransaction>> GetByUserAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(rt => rt.UserId == userId)
            .OrderByDescending(rt => rt.ProcessedAt)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<RelayTransaction>> GetUnprocessedAsync(CancellationToken cancellationToken = default)
    {
        // This would be for transactions pending processing
        // For now, return empty as all transactions are processed immediately
        return await Task.FromResult(Enumerable.Empty<RelayTransaction>());
    }
    
    public async Task<decimal> GetTotalCreditsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(rt => rt.UserId == userId)
            .SumAsync(rt => rt.CreditsEarned, cancellationToken);
    }
}