// MeshNetwork.Application/Commands/Handlers/RecordNetworkMetricCommandHandler.cs
using MediatR;
using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Core.Application.Commands.Handler;

public class RecordNetworkMetricCommandHandler : IRequestHandler<RecordNetworkMetricCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public RecordNetworkMetricCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Unit> Handle(RecordNetworkMetricCommand request, CancellationToken cancellationToken)
    {
        var metric = new NetworkMetric
        {
            Id = Guid.NewGuid(),
            NodeId = request.NodeId,
            Timestamp = DateTime.UtcNow,
            CpuUsage = request.CpuUsage,
            MemoryUsage = request.MemoryUsage,
            DiskUsage = request.DiskUsage,
            Temperature = request.Temperature,
            BytesSent = request.BytesSent,
            BytesReceived = request.BytesReceived,
            PacketsSent = 0,
            PacketsReceived = 0,
            PacketsDropped = 0,
            ActiveConnections = request.ActiveConnections,
            AverageLatency = request.AverageLatency,
            SignalStrength = request.SignalStrength
        };
        
        await _unitOfWork.Metrics.AddAsync(metric, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}