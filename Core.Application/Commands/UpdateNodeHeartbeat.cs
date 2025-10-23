using MediatR;

namespace Core.Application.Commands;

public record UpdateNodeHeartbeatCommand(
    Guid NodeId,
    double SignalStrength,
    double CpuUsage,
    double MemoryUsage,
    double Temperature
) : IRequest<Unit>;
