// MeshNetwork.Application/Queries/GetAllNodesQuery.cs
using MediatR;
using Core.Application.DTOs.Domain;

namespace Core.Application.Queries;

public record GetAllNodesQuery : IRequest<IEnumerable<NodeDto>>;