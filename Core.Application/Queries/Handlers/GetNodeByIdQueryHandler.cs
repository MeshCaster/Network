using AutoMapper;
using Core.Application.Commands.Handler;
using Core.Application.DTOs.Domain;
using Core.Domain.Interfaces;


namespace Core.Application.Queries.Handlers;

public class GetNodeByIdQueryHandler : IRequestHandler<GetNodeByIdQuery, NodeDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public GetNodeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<NodeDto?> Handle(GetNodeByIdQuery request, CancellationToken cancellationToken)
    {
        var node = await _unitOfWork.Nodes.GetByIdAsync(request.NodeId, cancellationToken);
        return node == null ? null : _mapper.Map<NodeDto>(node);
    }
}