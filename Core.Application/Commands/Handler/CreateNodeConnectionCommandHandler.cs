// MeshNetwork.Application/Commands/Handlers/CreateNodeConnectionCommandHandler.cs
using AutoMapper;
using Core.Application.DTOs.Domain;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces;

namespace Core.Application.Commands.Handler;

public class CreateNodeConnectionCommandHandler : IRequestHandler<CreateNodeConnectionCommand, NodeConnectionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public CreateNodeConnectionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<NodeConnectionDto> Handle(CreateNodeConnectionCommand request, CancellationToken cancellationToken)
    {
        // Check if connection already exists
        var existing = await _unitOfWork.Connections.GetConnectionAsync(
            request.SourceNodeId, 
            request.TargetNodeId, 
            cancellationToken);
        
        if (existing != null)
        {
            // Update existing connection
            existing.Quality = request.Quality;
            existing.Latency = request.Latency;
            existing.Throughput = request.Throughput;
            existing.PacketLoss = request.PacketLoss;
            existing.Rssi = request.Rssi;
            existing.LastUpdated = DateTime.UtcNow;
            existing.Status = ConnectionStatus.Active;
            
            await _unitOfWork.Connections.UpdateAsync(existing, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return _mapper.Map<NodeConnectionDto>(existing);
        }
        
        var connection = new NodeConnection
        {
            Id = Guid.NewGuid(),
            SourceNodeId = request.SourceNodeId,
            TargetNodeId = request.TargetNodeId,
            Quality = request.Quality,
            Latency = request.Latency,
            Throughput = request.Throughput,
            PacketLoss = request.PacketLoss,
            Rssi = request.Rssi,
            Status = ConnectionStatus.Active,
            EstablishedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            BytesSent = 0,
            BytesReceived = 0
        };
        
        await _unitOfWork.Connections.AddAsync(connection, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return _mapper.Map<NodeConnectionDto>(connection);
    }
}
