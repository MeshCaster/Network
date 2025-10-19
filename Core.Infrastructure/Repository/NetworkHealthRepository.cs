// MeshNetwork.Infrastructure/Data/Repositories/NetworkHealthRepository.cs
using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Core.Infrastructure.Data;

namespace Core.Infrastructure.Repository;

public class NetworkHealthRepository : Repository<NetworkHealth>, INetworkHealthRepository
{
    public NetworkHealthRepository(MeshDbContext context) : base(context) { }
    
    public async Task<NetworkHealth?> GetLatestAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .OrderByDescending(nh => nh.Timestamp)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<NetworkHealth>> GetHistoryAsync(
        DateTime from, 
        DateTime to, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(nh => nh.Timestamp >= from && nh.Timestamp <= to)
            .OrderBy(nh => nh.Timestamp)
            .ToListAsync(cancellationToken);
    }
}