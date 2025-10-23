using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces;

namespace Core.Application.Services;

/// <summary>
/// Service for calculating optimal routes through the mesh network using Dijkstra's algorithm
/// </summary>
public class RouteCalculationService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public RouteCalculationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    /// <summary>
    /// Calculate optimal route between two nodes
    /// </summary>
    public async Task<Route> CalculateRouteAsync(
        Guid sourceId, 
        Guid destinationId, 
        RoutingPreference preference = RoutingPreference.MinimumLatency,
        int maxHops = 5,
        CancellationToken cancellationToken = default)
    {
        // Check if cached valid route exists
        var cachedRoute = await _unitOfWork.Routes.GetValidRouteAsync(sourceId, destinationId, cancellationToken);
        if (cachedRoute != null)
        {
            cachedRoute.UsageCount++;
            cachedRoute.LastUsedAt = DateTime.UtcNow;
            await _unitOfWork.Routes.UpdateAsync(cachedRoute, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return cachedRoute;
        }
        
        // Verify nodes exist
        var sourceNode = await _unitOfWork.Nodes.GetByIdAsync(sourceId, cancellationToken);
        var destinationNode = await _unitOfWork.Nodes.GetByIdAsync(destinationId, cancellationToken);
        
        if (sourceNode == null)
            throw new NodeNotFoundException(sourceId);
        if (destinationNode == null)
            throw new NodeNotFoundException(destinationId);
        
        // Get all online nodes and healthy connections
        var allNodes = await _unitOfWork.Nodes.GetOnlineNodesAsync(cancellationToken);
        var allConnections = await _unitOfWork.Connections.GetHealthyConnectionsAsync(cancellationToken);
        
        // Build graph
        var graph = BuildGraph(allNodes, allConnections);
        
        // Run Dijkstra's algorithm
        var path = DijkstraShortestPath(graph, sourceId, destinationId, preference, maxHops);
        
        if (path == null || path.Count == 0)
            throw new RouteNotFoundException(sourceId, destinationId);
        
        // Calculate route metrics
        var route = await CreateRouteFromPath(path, preference, allConnections, cancellationToken);
        
        // Save route to cache
        await _unitOfWork.Routes.AddAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return route;
    }
    
    /// <summary>
    /// Build adjacency graph from nodes and connections
    /// </summary>
    private Dictionary<Guid, List<Edge>> BuildGraph(
        IEnumerable<Node> nodes,
        IEnumerable<NodeConnection> connections)
    {
        var graph = new Dictionary<Guid, List<Edge>>();
        
        foreach (var node in nodes)
        {
            graph[node.Id] = new List<Edge>();
        }
        
        foreach (var conn in connections)
        {
            if (graph.ContainsKey(conn.SourceNodeId) && graph.ContainsKey(conn.TargetNodeId))
            {
                // Bidirectional edges
                graph[conn.SourceNodeId].Add(new Edge
                {
                    TargetNodeId = conn.TargetNodeId,
                    Latency = conn.Latency,
                    Throughput = conn.Throughput,
                    Quality = conn.Quality,
                    PacketLoss = conn.PacketLoss
                });
                
                graph[conn.TargetNodeId].Add(new Edge
                {
                    TargetNodeId = conn.SourceNodeId,
                    Latency = conn.Latency,
                    Throughput = conn.Throughput,
                    Quality = conn.Quality,
                    PacketLoss = conn.PacketLoss
                });
            }
        }
        
        return graph;
    }
    
    /// <summary>
    /// Dijkstra's shortest path algorithm
    /// </summary>
    private List<Guid>? DijkstraShortestPath(
        Dictionary<Guid, List<Edge>> graph,
        Guid sourceId,
        Guid destinationId,
        RoutingPreference preference,
        int maxHops)
    {
        var distances = new Dictionary<Guid, double>();
        var previous = new Dictionary<Guid, Guid>();
        var visited = new HashSet<Guid>();
        var priorityQueue = new PriorityQueue<Guid, double>();
        
        // Initialize distances
        foreach (var nodeId in graph.Keys)
        {
            distances[nodeId] = double.MaxValue;
        }
        distances[sourceId] = 0;
        
        priorityQueue.Enqueue(sourceId, 0);
        
        while (priorityQueue.Count > 0)
        {
            var currentNode = priorityQueue.Dequeue();
            
            if (visited.Contains(currentNode))
                continue;
            
            visited.Add(currentNode);
            
            if (currentNode == destinationId)
                break;
            
            foreach (var edge in graph[currentNode])
            {
                if (visited.Contains(edge.TargetNodeId))
                    continue;
                
                var weight = CalculateEdgeWeight(edge, preference);
                var newDistance = distances[currentNode] + weight;
                
                if (newDistance < distances[edge.TargetNodeId])
                {
                    distances[edge.TargetNodeId] = newDistance;
                    previous[edge.TargetNodeId] = currentNode;
                    priorityQueue.Enqueue(edge.TargetNodeId, newDistance);
                }
            }
        }
        
        // Reconstruct path
        if (!previous.ContainsKey(destinationId))
            return null;
        
        var path = new List<Guid>();
        var current = destinationId;
        
        while (current != sourceId)
        {
            path.Add(current);
            if (!previous.ContainsKey(current))
                return null;
            current = previous[current];
        }
        path.Add(sourceId);
        path.Reverse();
        
        // Check hop count
        if (path.Count - 1 > maxHops)
            return null;
        
        return path;
    }
    
    /// <summary>
    /// Calculate edge weight based on routing preference
    /// </summary>
    private double CalculateEdgeWeight(Edge edge, RoutingPreference preference)
    {
        return preference switch
        {
            RoutingPreference.MinimumLatency => edge.Latency,
            RoutingPreference.MaximumThroughput => 1000000.0 / edge.Throughput, // Inverse for min-heap
            RoutingPreference.MinimumHops => 1.0,
            RoutingPreference.Balanced => (edge.Latency * 0.5) + ((100 - edge.Quality) * 0.5),
            _ => edge.Latency
        };
    }
    
    /// <summary>
    /// Create Route entity from calculated path
    /// </summary>
    private async Task<Route> CreateRouteFromPath(
        List<Guid> path,
        RoutingPreference preference,
        IEnumerable<NodeConnection> connections,
        CancellationToken cancellationToken)
    {
        var totalLatency = 0;
        var minThroughput = long.MaxValue;
        var qualitySum = 0.0;
        var hopCount = path.Count - 1;
        
        for (int i = 0; i < path.Count - 1; i++)
        {
            var connection = connections.FirstOrDefault(c =>
                (c.SourceNodeId == path[i] && c.TargetNodeId == path[i + 1]) ||
                (c.SourceNodeId == path[i + 1] && c.TargetNodeId == path[i]));
            
            if (connection != null)
            {
                totalLatency += connection.Latency;
                minThroughput = Math.Min(minThroughput, connection.Throughput);
                qualitySum += connection.Quality;
            }
        }
        
        var avgQuality = hopCount > 0 ? qualitySum / hopCount : 0;
        
        return new Route
        {
            Id = Guid.NewGuid(),
            SourceNodeId = path.First(),
            DestinationNodeId = path.Last(),
            PathNodeIds = path,
            HopCount = hopCount,
            TotalLatency = totalLatency,
            MinThroughput = minThroughput,
            AverageQuality = avgQuality,
            Preference = preference,
            CalculatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsValid = true,
            UsageCount = 1,
            LastUsedAt = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Invalidate routes that include a specific node
    /// </summary>
    public async Task InvalidateRoutesForNodeAsync(Guid nodeId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Routes.InvalidateRoutesAsync(nodeId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    private class Edge
    {
        public Guid TargetNodeId { get; set; }
        public int Latency { get; set; }
        public long Throughput { get; set; }
        public double Quality { get; set; }
        public double PacketLoss { get; set; }
    }
}