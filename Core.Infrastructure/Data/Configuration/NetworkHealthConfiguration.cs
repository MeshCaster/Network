// MeshNetwork.Infrastructure/Data/Configurations/NetworkHealthConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Domain.Entities;

namespace Core.Infrastructure.Data.Configuration;

public class NetworkHealthConfiguration : IEntityTypeConfiguration<NetworkHealth>
{
    public void Configure(EntityTypeBuilder<NetworkHealth> builder)
    {
        builder.ToTable("network_health");
        
        builder.HasKey(nh => nh.Id);
        
        builder.Property(nh => nh.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        builder.Property(nh => nh.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();
        
        builder.Property(nh => nh.OverallScore)
            .HasColumnName("overall_score")
            .HasPrecision(5, 2);
        
        builder.Property(nh => nh.NodeAvailabilityScore)
            .HasColumnName("node_availability_score")
            .HasPrecision(5, 2);
        
        builder.Property(nh => nh.ConnectionQualityScore)
            .HasColumnName("connection_quality_score")
            .HasPrecision(5, 2);
        
        builder.Property(nh => nh.RouteRedundancyScore)
            .HasColumnName("route_redundancy_score")
            .HasPrecision(5, 2);
        
        builder.Property(nh => nh.GatewayAvailabilityScore)
            .HasColumnName("gateway_availability_score")
            .HasPrecision(5, 2);
        
        builder.Property(nh => nh.TotalNodes)
            .HasColumnName("total_nodes");
        
        builder.Property(nh => nh.OnlineNodes)
            .HasColumnName("online_nodes");
        
        builder.Property(nh => nh.TotalConnections)
            .HasColumnName("total_connections");
        
        builder.Property(nh => nh.HealthyConnections)
            .HasColumnName("healthy_connections");
        
        builder.Property(nh => nh.AvailableGateways)
            .HasColumnName("available_gateways");
        
        builder.Property(nh => nh.TotalRoutes)
            .HasColumnName("total_routes");
        
        // Indexes
        builder.HasIndex(nh => nh.Timestamp);
    }
}