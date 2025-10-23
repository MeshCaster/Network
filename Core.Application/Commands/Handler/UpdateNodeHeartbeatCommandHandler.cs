// MeshNetwork.Application/Commands/Handlers/UpdateNodeHeartbeatCommandHandler.cs
using MediatR;
using Core.Application.Services;

namespace Core.Application.Commands.Handler;

public class UpdateNodeHeartbeatCommandHandler : IRequestHandler<UpdateNodeHeartbeatCommand, Unit>
{
    private readonly NodeManagementService _nodeService;
    
    public UpdateNodeHeartbeatCommandHandler(NodeManagementService nodeService)
    {
        _nodeService = nodeService;
    }
    
    public async Task<Unit> Handle(UpdateNodeHeartbeatCommand request, CancellationToken cancellationToken)
    {
        await _nodeService.UpdateNodeHeartbeatAsync(
            request.NodeId,
            request.SignalStrength,
            request.CpuUsage,
            request.MemoryUsage,
            request.Temperature,
            cancellationToken);
        
        return Unit.Value;
    }
}