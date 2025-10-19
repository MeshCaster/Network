// MeshNetwork.Infrastructure/Data/Configurations/RouteConfiguration.cs
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Data.Configuration;

public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ToTable("routes");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.SourceNodeId)
            .IsRequired();
        
        builder.Property(r => r.DestinationNodeId)
            .IsRequired();
        
        builder.Property(r => r.PathNodeIds)
            .HasColumnType("jsonb")
            .IsRequired();
        
        builder.Property(r => r.HopCount)
            .IsRequired();
        
        builder.Property(r => r.TotalLatency)
            .IsRequired();
        
        builder.Property(r => r.MinThroughput)
            .IsRequired();
        
        builder.Property(r => r.AverageQuality)
            .HasPrecision(5, 2);
        
        builder.Property(r => r.Preference)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(r => r.CalculatedAt)
            .IsRequired();
        
        builder.Property(r => r.ExpiresAt)
            .IsRequired();
        
        builder.Property(r => r.IsValid)
            .HasDefaultValue(true);
        
        builder.Property(r => r.UsageCount)
            .HasDefaultValue(0);
        
        // Relationships
        builder.HasOne(r => r.SourceNode)
            .WithMany()
            .HasForeignKey(r => r.SourceNodeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(r => r.DestinationNode)
            .WithMany()
            .HasForeignKey(r => r.DestinationNodeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(r => new { r.SourceNodeId, r.DestinationNodeId, r.IsValid });
        
        builder.HasIndex(r => r.ExpiresAt);
        
        builder.HasIndex(r => r.IsValid);
    }
}