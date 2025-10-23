using System.Text;
using Core.Application.Commands;
using Core.Application.Hubs;
using Core.Application.Jobs;
using Core.Application.Services;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Core.Application;

public static class DI
{
    public static void AddNodeManagement(this IServiceCollection services, IConfiguration configuration)
    {
        // Application Services
        services.AddScoped<NodeManagementService>();
        services.AddScoped<RouteCalculationService>();
        services.AddScoped<NetworkHealthService>();
        services.AddScoped<RelayCreditService>();
        services.AddScoped<AuthService>();
        services.AddScoped<NetworkHubService>();
    }

    public static void AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        // Background Jobs
        services.AddScoped<RouteRefreshJob>();
        services.AddScoped<HealthCheckJob>();
        services.AddScoped<NetworkHealthCalculationJob>();
        services.AddScoped<MetricsCleanupJob>();
        services.AddScoped<RelayCreditCalculationJob>();
    }

    public static void AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        // Health Checks
    }
    
    public static void AddMediatR(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterNodeCommand).Assembly));
    }

    public static void AddHub(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSignalR();
    }

    public static void ConfigureHubs(this WebApplication application, IConfiguration configuration)
    {
        application.MapHub<NetworkHub>("/hubs/network");
    }

    public static void ConfigureHangfireJobs(this WebApplication app, IConfiguration configuration)
    {
        // Hangfire Dashboard
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
        });
        
        // Configure recurring jobs
        JobScheduler.ConfigureRecurringJobs();
    }

    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        // JWT Authentication
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
    
                // Configure JWT for SignalR
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
            
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
            
                        return Task.CompletedTask;
                    }
                };
            });
        
        services.AddAuthorization();
    }
}

// Hangfire Authorization Filter (Allow all in development, secure in production)
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In production, implement proper authorization
        return true;
    }
}