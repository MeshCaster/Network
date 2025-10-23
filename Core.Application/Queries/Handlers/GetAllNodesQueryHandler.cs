using AutoMapper;
using Core.Application.Commands.Handler;
using Core.Application.DTOs.Domain;
using Core.Domain.Interfaces;

namespace Core.Application.Queries.Handlers;

public class GetAllNodesQueryHandler : IRequestHandler<GetAllNodesQuery, IEnumerable<NodeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public GetAllNodesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<NodeDto>> Handle(GetAllNodesQuery request, CancellationToken cancellationToken)
    {
        var nodes = await _unitOfWork.Nodes.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<NodeDto>>(nodes);
    }
}
