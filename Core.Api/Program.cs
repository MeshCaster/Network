// MeshNetwork.API/Program.cs
using System.Text;
using Core.Application.Commands;
using Core.Application.Hubs;
using Core.Application.Jobs;
using Core.Application.Mappings;
using Core.Application.Services;
using Core.Application.Validators;
using Core.Domain.Interfaces;
using Core.Infrastructure.Data;
using Core.Infrastructure.Repository;
using FluentValidation;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        "logs/meshnetwork-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        retainedFileCountLimit: 30) // Keep 30 days of logs
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
var services = builder.Services;
var configuration = builder.Configuration;

// Database Configuration
services.AddDbContext<MeshDbContext>(options =>
{
    options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.UseNetTopologySuite();
            npgsqlOptions.MigrationsAssembly("MeshNetwork.Infrastructure");
        });
});

// Redis Cache
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("Redis");
    options.InstanceName = "MeshNetwork_";
});

// Repository Registration
services.AddScoped<INodeRepository, NodeRepository>();
services.AddScoped<INodeConnectionRepository, NodeConnectionRepository>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IRouteRepository, RouteRepository>();
services.AddScoped<INetworkMetricRepository, NetworkMetricRepository>();
services.AddScoped<IRelayTransactionRepository, RelayTransactionRepository>();
services.AddScoped<INetworkHealthRepository, NetworkHealthRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application Services
services.AddScoped<NodeManagementService>();
services.AddScoped<RouteCalculationService>();
services.AddScoped<NetworkHealthService>();
services.AddScoped<RelayCreditService>();
services.AddScoped<AuthService>();
services.AddScoped<NetworkHubService>();

// Background Jobs
services.AddScoped<RouteRefreshJob>();
services.AddScoped<HealthCheckJob>();
services.AddScoped<NetworkHealthCalculationJob>();
services.AddScoped<MetricsCleanupJob>();
services.AddScoped<RelayCreditCalculationJob>();

// AutoMapper
services.AddAutoMapper(typeof(MappingProfile));

// MediatR
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterNodeCommand).Assembly));

// FluentValidation
services.AddValidatorsFromAssemblyContaining<RegisterNodeCommandValidator>();

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

// SignalR
services.AddSignalR();

// Hangfire
services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));
    config.UseSimpleAssemblyNameTypeSerializer();
    config.UseRecommendedSerializerSettings();
});

services.AddHangfireServer(options =>
{
    options.WorkerCount = 5;
});

// Controllers
services.AddControllers();

// CORS
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger/OpenAPI
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mesh Network Management API",
        Version = "v1",
        Description = "API for managing city-wide mesh network topology and routing"
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Health Checks
services.AddHealthChecks()
    .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(configuration.GetConnectionString("Redis")!);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mesh Network API V1");
    });
}

// Apply pending migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MeshDbContext>();
    dbContext.Database.Migrate();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR Hub
app.MapHub<NetworkHub>("/hubs/network");

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// Configure recurring jobs
JobScheduler.ConfigureRecurringJobs();

// Health check endpoint
app.MapHealthChecks("/health");

Log.Information("Mesh Network Management API starting...");

app.Run();

Log.Information("Mesh Network Management API stopped");

// Hangfire Authorization Filter (Allow all in development, secure in production)
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In production, implement proper authorization
        return true;
    }
}