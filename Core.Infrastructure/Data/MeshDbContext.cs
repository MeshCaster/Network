// MeshNetwork.Infrastructure/Data/MeshDbContext.cs

using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Data;

public class MeshDbContext : DbContext
{
    public MeshDbContext(DbContextOptions<MeshDbContext> options) : base(options) { }
    
    public DbSet<Node> Nodes => Set<Node>();
    public DbSet<NodeConnection> NodeConnections => Set<NodeConnection>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<NetworkMetric> NetworkMetrics => Set<NetworkMetric>();
    public DbSet<RelayTransaction> RelayTransactions => Set<RelayTransaction>();
    public DbSet<NetworkHealth> NetworkHealthSnapshots => Set<NetworkHealth>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeshDbContext).Assembly);
        
        // Enable PostGIS extension for geospatial operations
        modelBuilder.HasPostgresExtension("postgis");
    }
    
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        
        // Use snake_case naming convention (optional but recommended for PostgreSQL)
        configurationBuilder.Conventions.Add(_ => new SnakeCaseNamingConvention());
    }
}