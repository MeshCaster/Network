// MeshNetwork.Application/Commands/RecordNetworkMetricCommand.cs
using MediatR;

namespace Core.Application.Commands;

public record RecordNetworkMetricCommand(
    Guid NodeId,
    double CpuUsage,
    double MemoryUsage,
    double DiskUsage,
    double Temperature,
    long BytesSent,
    long BytesReceived,
    int ActiveConnections,
    double AverageLatency,
    double SignalStrength
) : IRequest<Unit>;