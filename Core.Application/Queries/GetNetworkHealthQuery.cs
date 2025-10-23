using Core.Application.Commands;
using Core.Application.DTOs.Domain;

namespace Core.Application.Queries;

public record GetNetworkHealthQuery : IRequest<NetworkHealthDto?>;
