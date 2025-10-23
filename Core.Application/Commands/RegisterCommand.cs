// MeshNetwork.Application/Commands/RegisterNodeCommand.cs
using MediatR;
using Core.Application.DTOs.Domain;
using Core.Domain.Enums;

namespace Core.Application.Commands;

public interface IRequest<T>{}

public record RegisterNodeCommand(
    string Name,
    NodeType Type,
    double Latitude,
    double Longitude,
    string IpAddress,
    string MacAddress,
    long BandwidthCapacity,
    Guid? OwnerId = null
) : IRequest<NodeDto>;