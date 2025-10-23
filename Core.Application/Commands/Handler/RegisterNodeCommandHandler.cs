// MeshNetwork.Application/Commands/Handlers/RegisterNodeCommandHandler.cs
using AutoMapper;
using Core.Application.DTOs.Domain;
using Core.Application.Services;

namespace Core.Application.Commands.Handler;

public interface IRequestHandler<T1, T2>;

public class RegisterNodeCommandHandler : IRequestHandler<RegisterNodeCommand, NodeDto>
{
    private readonly NodeManagementService _nodeService;
    private readonly IMapper _mapper;
    
    public RegisterNodeCommandHandler(NodeManagementService nodeService, IMapper mapper)
    {
        _nodeService = nodeService;
        _mapper = mapper;
    }
    
    public async Task<NodeDto> Handle(RegisterNodeCommand request, CancellationToken cancellationToken)
    {
        var node = await _nodeService.RegisterNodeAsync(
            request.Name,
            request.Type,
            request.Latitude,
            request.Longitude,
            request.IpAddress,
            request.MacAddress,
            request.BandwidthCapacity,
            request.OwnerId,
            cancellationToken);
        
        return _mapper.Map<NodeDto>(node);
    }
}