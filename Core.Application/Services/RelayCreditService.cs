using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Core.Application.Services;

/// <summary>
/// Service for calculating relay credits earned by users
/// </summary>
public class RelayCreditService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public RelayCreditService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    /// <summary>
    /// Calculate and award credits for relay activity
    /// </summary>
    public async Task<RelayTransaction> CalculateAndAwardCreditsAsync(
        Guid userId,
        Guid nodeId,
        long bytesRelayed,
        double connectionQuality,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default)
    {
        // Base credit rate: 1 credit per GB relayed
        const decimal baseRatePerGb = 1.0m;
        var gigabytesRelayed = bytesRelayed / (1024.0 * 1024.0 * 1024.0);
        var baseCredits = (decimal)gigabytesRelayed * baseRatePerGb;
        
        // Quality multiplier (0.5x to 2x based on connection quality)
        var qualityMultiplier = CalculateQualityMultiplier(connectionQuality);
        
        // Time of day multiplier (1x to 1.5x for peak hours)
        var timeOfDayMultiplier = CalculateTimeOfDayMultiplier(startTime, endTime);
        
        // Calculate final credits
        var creditsEarned = baseCredits * (decimal)qualityMultiplier * (decimal)timeOfDayMultiplier;
        
        // Create transaction record
        var transaction = new RelayTransaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NodeId = nodeId,
            BytesRelayed = bytesRelayed,
            CreditsEarned = Math.Round(creditsEarned, 2),
            StartTime = startTime,
            EndTime = endTime,
            QualityMultiplier = qualityMultiplier,
            TimeOfDayMultiplier = timeOfDayMultiplier,
            ProcessedAt = DateTime.UtcNow
        };
        
        await _unitOfWork.RelayTransactions.AddAsync(transaction, cancellationToken);
        
        // Update user's total credits and bytes relayed
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user != null)
        {
            user.RelayCredits += creditsEarned;
            user.TotalBytesRelayed += bytesRelayed;
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return transaction;
    }
    
    /// <summary>
    /// Calculate quality multiplier based on connection quality
    /// </summary>
    private double CalculateQualityMultiplier(double quality)
    {
        // Quality 0-100 maps to multiplier 0.5-2.0
        return 0.5 + (quality / 100.0 * 1.5);
    }
    
    /// <summary>
    /// Calculate time of day multiplier (peak hours get bonus)
    /// </summary>
    private double CalculateTimeOfDayMultiplier(DateTime startTime, DateTime endTime)
    {
        // Peak hours: 6 PM to 11 PM (18:00 to 23:00)
        var avgTime = startTime.AddSeconds((endTime - startTime).TotalSeconds / 2);
        var hour = avgTime.Hour;
        
        if (hour >= 18 && hour < 23)
            return 1.5; // Peak hours bonus
        
        return 1.0; // Normal hours
    }
    
    /// <summary>
    /// Get user's relay statistics
    /// </summary>
    public async Task<RelayStatistics> GetUserRelayStatisticsAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new Exception("User not found");
        
        var transactions = await _unitOfWork.RelayTransactions.GetByUserAsync(userId, cancellationToken);
        var transactionList = transactions.ToList();
        
        return new RelayStatistics
        {
            TotalCreditsEarned = user.RelayCredits,
            TotalBytesRelayed = user.TotalBytesRelayed,
            TransactionCount = transactionList.Count,
            AverageQualityMultiplier = transactionList.Any() 
                ? transactionList.Average(t => t.QualityMultiplier) 
                : 0,
            LastTransactionDate = transactionList.Any() 
                ? transactionList.Max(t => t.ProcessedAt) 
                : (DateTime?)null
        };
    }
    
    public class RelayStatistics
    {
        public decimal TotalCreditsEarned { get; set; }
        public long TotalBytesRelayed { get; set; }
        public int TransactionCount { get; set; }
        public double AverageQualityMultiplier { get; set; }
        public DateTime? LastTransactionDate { get; set; }
    }
}
