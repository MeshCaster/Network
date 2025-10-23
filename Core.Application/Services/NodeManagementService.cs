using Core.Application.Services;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;
using NetTopologySuite.Geometries;

namespace Core.Application.Services;

/// <summary>
/// Service for managing mesh network nodes
/// </summary>
public class NodeManagementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly RouteCalculationService _routeService;
    
    public NodeManagementService(
        IUnitOfWork unitOfWork,
        RouteCalculationService routeService)
    {
        _unitOfWork = unitOfWork;
        _routeService = routeService;
    }
    
    /// <summary>
    /// Register a new node in the network
    /// </summary>
    public async Task<Node> RegisterNodeAsync(
        string name,
        NodeType type,
        double latitude,
        double longitude,
        string ipAddress,
        string macAddress,
        long bandwidthCapacity,
        Guid? ownerId = null,
        CancellationToken cancellationToken = default)
    {
        // Check if MAC address already exists
        var existingNode = await _unitOfWork.Nodes.GetByMacAddressAsync(macAddress, cancellationToken);
        if (existingNode != null)
            throw new InvalidOperationException($"Node with MAC address {macAddress} already exists");
        
        // Create location point
        var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var location = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
        
        var node = new Node
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            Status = NodeStatus.Online,
            Location = location,
            IpAddress = ipAddress,
            MacAddress = macAddress,
            BandwidthCapacity = bandwidthCapacity,
            SignalStrength = 100,
            CpuUsage = 0,
            MemoryUsage = 0,
            Temperature = 0,
            CreatedAt = DateTime.UtcNow,
            LastHeartbeat = DateTime.UtcNow,
            OwnerId = ownerId
        };
        
        await _unitOfWork.Nodes.AddAsync(node, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return node;
    }
    
    /// <summary>
    /// Update node heartbeat and status
    /// </summary>
    public async Task UpdateNodeHeartbeatAsync(
        Guid nodeId,
        double signalStrength,
        double cpuUsage,
        double memoryUsage,
        double temperature,
        CancellationToken cancellationToken = default)
    {
        var node = await _unitOfWork.Nodes.GetByIdAsync(nodeId, cancellationToken);
        if (node == null)
            throw new NodeNotFoundException(nodeId);
        
        var previousStatus = node.Status;
        
        node.LastHeartbeat = DateTime.UtcNow;
        node.SignalStrength = signalStrength;
        node.CpuUsage = cpuUsage;
        node.MemoryUsage = memoryUsage;
        node.Temperature = temperature;
        node.Status = DetermineNodeStatus(signalStrength, cpuUsage, memoryUsage);
        
        await _unitOfWork.Nodes.UpdateAsync(node, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        // If node status changed to offline, invalidate routes
        if (previousStatus == NodeStatus.Online && node.Status != NodeStatus.Online)
        {
            await _routeService.InvalidateRoutesForNodeAsync(nodeId, cancellationToken);
        }
    }
    
    /// <summary>
    /// Determine node status based on metrics
    /// </summary>
    private NodeStatus DetermineNodeStatus(double signalStrength, double cpuUsage, double memoryUsage)
    {
        if (signalStrength < 20 || cpuUsage > 95 || memoryUsage > 95)
            return NodeStatus.Degraded;
        
        return NodeStatus.Online;
    }
    
    /// <summary>
    /// Mark stale nodes as offline
    /// </summary>
    public async Task MarkStaleNodesOfflineAsync(int timeoutSeconds = 300, CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddSeconds(-timeoutSeconds);
        var allNodes = await _unitOfWork.Nodes.GetAllAsync(cancellationToken);
        
        foreach (var node in allNodes.Where(n => n.Status == NodeStatus.Online && n.LastHeartbeat < threshold))
        {
            node.Status = NodeStatus.Offline;
            node.LastOfflineAt = DateTime.UtcNow;
            await _unitOfWork.Nodes.UpdateAsync(node, cancellationToken);
            await _routeService.InvalidateRoutesForNodeAsync(node.Id, cancellationToken);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    /// <summary>
    /// Get nodes within radius of a location
    /// </summary>
    public async Task<IEnumerable<Node>> GetNodesNearLocationAsync(
        double latitude,
        double longitude,
        double radiusMeters,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Nodes.GetWithinRadiusAsync(latitude, longitude, radiusMeters, cancellationToken);
    }
    
    /// <summary>
    /// Delete a node and its connections
    /// </summary>
    public async Task DeleteNodeAsync(Guid nodeId, CancellationToken cancellationToken = default)
    {
        var node = await _unitOfWork.Nodes.GetByIdAsync(nodeId, cancellationToken);
        if (node == null)
            throw new NodeNotFoundException(nodeId);
        
        await _routeService.InvalidateRoutesForNodeAsync(nodeId, cancellationToken);
        await _unitOfWork.Nodes.DeleteAsync(node, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}