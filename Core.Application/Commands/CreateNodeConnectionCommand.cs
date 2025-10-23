// MeshNetwork.Application/Commands/CreateNodeConnectionCommand.cs
using Core.Application.DTOs.Domain;

namespace Core.Application.Commands;

public record CreateNodeConnectionCommand(
    Guid SourceNodeId,
    Guid TargetNodeId,
    double Quality,
    int Latency,
    long Throughput,
    double PacketLoss,
    int Rssi
) : IRequest<NodeConnectionDto>;