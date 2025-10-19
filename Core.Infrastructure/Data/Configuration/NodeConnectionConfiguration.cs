// MeshNetwork.Infrastructure/Data/Configurations/NodeConnectionConfiguration.cs
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Data.Configuration;

public class NodeConnectionConfiguration : IEntityTypeConfiguration<NodeConnection>
{
    public void Configure(EntityTypeBuilder<NodeConnection> builder)
    {
        builder.ToTable("node_connections");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.SourceNodeId)
            .IsRequired();
        
        builder.Property(c => c.TargetNodeId)
            .IsRequired();
        
        builder.Property(c => c.Quality)
            .HasPrecision(5, 2)
            .IsRequired();
        
        builder.Property(c => c.Latency)
            .IsRequired();
        
        builder.Property(c => c.Throughput)
            .IsRequired();
        
        builder.Property(c => c.PacketLoss)
            .HasPrecision(5, 2);
        
        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(c => c.EstablishedAt)
            .IsRequired();
        
        builder.Property(c => c.LastUpdated)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(c => c.SourceNodeId);
        
        builder.HasIndex(c => c.TargetNodeId);
        
        builder.HasIndex(c => c.Status);
        
        builder.HasIndex(c => new { c.SourceNodeId, c.TargetNodeId })
            .IsUnique();
    }
}
