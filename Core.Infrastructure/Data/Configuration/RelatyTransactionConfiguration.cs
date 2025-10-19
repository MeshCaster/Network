using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Domain.Entities;

namespace Core.Infrastructure.Data.Configuration;

public class RelayTransactionConfiguration : IEntityTypeConfiguration<RelayTransaction>
{
    public void Configure(EntityTypeBuilder<RelayTransaction> builder)
    {
        builder.ToTable("relay_transactions");
        
        builder.HasKey(rt => rt.Id);
        
        builder.Property(rt => rt.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        builder.Property(rt => rt.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        builder.Property(rt => rt.NodeId)
            .HasColumnName("node_id")
            .IsRequired();
        
        builder.Property(rt => rt.BytesRelayed)
            .HasColumnName("bytes_relayed")
            .IsRequired();
        
        builder.Property(rt => rt.CreditsEarned)
            .HasColumnName("credits_earned")
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(rt => rt.StartTime)
            .HasColumnName("start_time")
            .IsRequired();
        
        builder.Property(rt => rt.EndTime)
            .HasColumnName("end_time")
            .IsRequired();
        
        builder.Property(rt => rt.QualityMultiplier)
            .HasColumnName("quality_multiplier")
            .HasPrecision(5, 2);
        
        builder.Property(rt => rt.TimeOfDayMultiplier)
            .HasColumnName("time_of_day_multiplier")
            .HasPrecision(5, 2);
        
        builder.Property(rt => rt.ProcessedAt)
            .HasColumnName("processed_at")
            .IsRequired();
        
        // Relationships
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RelayTransactions)
            .HasForeignKey(rt => rt.UserId);
        
        builder.HasOne(rt => rt.Node)
            .WithMany()
            .HasForeignKey(rt => rt.NodeId);
        
        // Indexes
        builder.HasIndex(rt => rt.UserId);
        builder.HasIndex(rt => rt.NodeId);
        builder.HasIndex(rt => rt.ProcessedAt);
    }
}