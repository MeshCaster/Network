// MeshNetwork.Infrastructure/Data/Configurations/NodeConfiguration.cs
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Infrastructure.Data.Configuration;

public class NodeConfiguration : IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        builder.ToTable("nodes");
        
        builder.HasKey(n => n.Id);
        
        // Properties with column names (EF Core 9 syntax)
        builder.Property(n => n.Id)
            .HasColumnName("id");
        
        builder.Property(n => n.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(n => n.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(n => n.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
        
        // Geospatial column (EF Core 9 syntax for PostGIS)
        builder.Property(n => n.Location)
            .HasColumnName("location")
            .HasColumnType("geography(Point, 4326)")
            .IsRequired();
        
        builder.Property(n => n.SignalStrength)
            .HasColumnName("signal_strength")
            .HasPrecision(5, 2);
        
        builder.Property(n => n.BandwidthCapacity)
            .HasColumnName("bandwidth_capacity");
        
        builder.Property(n => n.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45);
        
        builder.Property(n => n.MacAddress)
            .HasColumnName("mac_address")
            .HasMaxLength(17);
        
        builder.Property(n => n.CpuUsage)
            .HasColumnName("cpu_usage")
            .HasPrecision(5, 2);
        
        builder.Property(n => n.MemoryUsage)
            .HasColumnName("memory_usage")
            .HasPrecision(5, 2);
        
        builder.Property(n => n.Temperature)
            .HasColumnName("temperature")
            .HasPrecision(5, 2);
        
        builder.Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        
        builder.Property(n => n.LastHeartbeat)
            .HasColumnName("last_heartbeat")
            .IsRequired();
        
        builder.Property(n => n.LastOfflineAt)
            .HasColumnName("last_offline_at");
        
        builder.Property(n => n.OwnerId)
            .HasColumnName("owner_id");
        
        // Relationships
        builder.HasOne(n => n.Owner)
            .WithMany(u => u.Nodes)
            .HasForeignKey(n => n.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(n => n.OutgoingConnections)
            .WithOne(c => c.SourceNode)
            .HasForeignKey(c => c.SourceNodeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(n => n.IncomingConnections)
            .WithOne(c => c.TargetNode)
            .HasForeignKey(c => c.TargetNodeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(n => n.Metrics)
            .WithOne(m => m.Node)
            .HasForeignKey(m => m.NodeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes (EF Core 9 syntax)
        builder.HasIndex(n => n.MacAddress)
            .IsUnique();
        
        builder.HasIndex(n => n.Status);
        
        builder.HasIndex(n => n.Type);
        
        builder.HasIndex(n => n.OwnerId);
        
        builder.HasIndex(n => n.LastHeartbeat);
        
        // Spatial index (EF Core 9 with PostGIS)
        // Use raw SQL for spatial indexes as HasMethod is not available
        builder.HasIndex(n => n.Location)
            .HasDatabaseName("IX_nodes_location");
    }
}
