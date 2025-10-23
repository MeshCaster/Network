using Core.Domain.Interfaces;
using Core.Infrastructure.Data;
using Core.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure;

public static class DI
{
    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Configuration
        services.AddDbContext<MeshDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly("MeshNetwork.Infrastructure");
                });
        });   
    }

    public static void AddRepository(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INodeRepository, NodeRepository>();
        services.AddScoped<INodeConnectionRepository, NodeConnectionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRouteRepository, RouteRepository>();
        services.AddScoped<INetworkMetricRepository, NetworkMetricRepository>();
        services.AddScoped<IRelayTransactionRepository, RelayTransactionRepository>();
        services.AddScoped<INetworkHealthRepository, NetworkHealthRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}