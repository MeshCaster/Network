// MeshNetwork.API/Hubs/NetworkHub.cs

using Core.Application.DTOs.Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Hubs;

/// <summary>
/// SignalR hub for real-time network updates
/// </summary>
public class NetworkHub : Hub
{
    private readonly ILogger<NetworkHub> _logger;
    
    public NetworkHub(ILogger<NetworkHub> logger)
    {
        _logger = logger;
    }
    
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
    
    /// <summary>
    /// Subscribe to node updates
    /// </summary>
    public async Task SubscribeToNode(string nodeId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"node_{nodeId}");
        _logger.LogInformation("Client {ConnectionId} subscribed to node {NodeId}", Context.ConnectionId, nodeId);
    }
    
    /// <summary>
    /// Unsubscribe from node updates
    /// </summary>
    public async Task UnsubscribeFromNode(string nodeId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"node_{nodeId}");
        _logger.LogInformation("Client {ConnectionId} unsubscribed from node {NodeId}", Context.ConnectionId, nodeId);
    }
    
    /// <summary>
    /// Subscribe to network health updates
    /// </summary>
    public async Task SubscribeToNetworkHealth()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "network_health");
        _logger.LogInformation("Client {ConnectionId} subscribed to network health", Context.ConnectionId);
    }
}

/// <summary>
/// Service for broadcasting updates via SignalR
/// </summary>
public class NetworkHubService
{
    private readonly IHubContext<NetworkHub> _hubContext;
    
    public NetworkHubService(IHubContext<NetworkHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task BroadcastNodeStatusChangedAsync(NodeDto node)
    {
        await _hubContext.Clients.Group($"node_{node.Id}")
            .SendAsync("NodeStatusChanged", node);
    }
    
    public async Task BroadcastRouteUpdatedAsync(RouteDto route)
    {
        await _hubContext.Clients.All
            .SendAsync("RouteUpdated", route);
    }
    
    public async Task BroadcastMetricReceivedAsync(NetworkMetricDto metric)
    {
        await _hubContext.Clients.Group($"node_{metric.NodeId}")
            .SendAsync("MetricReceived", metric);
    }
    
    public async Task BroadcastNetworkHealthAsync(NetworkHealthDto health)
    {
        await _hubContext.Clients.Group("network_health")
            .SendAsync("NetworkHealthUpdated", health);
    }
    
    public async Task BroadcastConnectionStatusAsync(NodeConnectionDto connection)
    {
        await _hubContext.Clients.All
            .SendAsync("ConnectionStatusChanged", connection);
    }
}