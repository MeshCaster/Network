// MeshNetwork.Infrastructure/Data/Configurations/NetworkMetricConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Domain.Entities;

namespace MeshNetwork.Infrastructure.Data.Configuration;

public class NetworkMetricConfiguration : IEntityTypeConfiguration<NetworkMetric>
{
    public void Configure(EntityTypeBuilder<NetworkMetric> builder)
    {
        builder.ToTable("network_metrics");
        
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        builder.Property(m => m.NodeId)
            .HasColumnName("node_id")
            .IsRequired();
        
        builder.Property(m => m.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();
        
        builder.Property(m => m.CpuUsage)
            .HasColumnName("cpu_usage")
            .HasPrecision(5, 2);
        
        builder.Property(m => m.MemoryUsage)
            .HasColumnName("memory_usage")
            .HasPrecision(5, 2);
        
        builder.Property(m => m.DiskUsage)
            .HasColumnName("disk_usage")
            .HasPrecision(5, 2);
        
        builder.Property(m => m.Temperature)
            .HasColumnName("temperature")
            .HasPrecision(5, 2);
        
        builder.Property(m => m.BytesSent)
            .HasColumnName("bytes_sent");
        
        builder.Property(m => m.BytesReceived)
            .HasColumnName("bytes_received");
        
        builder.Property(m => m.PacketsSent)
            .HasColumnName("packets_sent");
        
        builder.Property(m => m.PacketsReceived)
            .HasColumnName("packets_received");
        
        builder.Property(m => m.PacketsDropped)
            .HasColumnName("packets_dropped");
        
        builder.Property(m => m.ActiveConnections)
            .HasColumnName("active_connections");
        
        builder.Property(m => m.AverageLatency)
            .HasColumnName("average_latency")
            .HasPrecision(8, 2);
        
        builder.Property(m => m.SignalStrength)
            .HasColumnName("signal_strength")
            .HasPrecision(5, 2);
        
        // Relationships
        builder.HasOne(m => m.Node)
            .WithMany(n => n.Metrics)
            .HasForeignKey(m => m.NodeId);
        
        // Indexes for time-series queries
        builder.HasIndex(m => new { m.NodeId, m.Timestamp });
        builder.HasIndex(m => m.Timestamp);
    }
}