using Core.Application.DTOs.Domain;
using Core.Domain.Enums;


namespace Core.Application.Commands;

public record CalculateRouteCommand(
    Guid SourceNodeId,
    Guid DestinationNodeId,
    RoutingPreference Preference = RoutingPreference.MinimumLatency,
    int MaxHops = 5
) : IRequest<RouteDto>;
