// MeshNetwork.Application/Commands/Handlers/CalculateRouteCommandHandler.cs
using AutoMapper;
using Core.Application.DTOs.Domain;
using Core.Application.Services;
using Core.Domain.Interfaces;

namespace Core.Application.Commands.Handler;

public class CalculateRouteCommandHandler : IRequestHandler<CalculateRouteCommand, RouteDto>
{
    private readonly RouteCalculationService _routeService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public CalculateRouteCommandHandler(
        RouteCalculationService routeService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _routeService = routeService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<RouteDto> Handle(CalculateRouteCommand request, CancellationToken cancellationToken)
    {
        var route = await _routeService.CalculateRouteAsync(
            request.SourceNodeId,
            request.DestinationNodeId,
            request.Preference,
            request.MaxHops,
            cancellationToken);
        
        var routeDto = _mapper.Map<RouteDto>(route);
        
        // Get node names for path
        var pathNodeNames = new List<string>();
        foreach (var nodeId in route.PathNodeIds)
        {
            var node = await _unitOfWork.Nodes.GetByIdAsync(nodeId, cancellationToken);
            pathNodeNames.Add(node?.Name ?? "Unknown");
        }
        routeDto.PathNodeNames = pathNodeNames;
        
        return routeDto;
    }
}