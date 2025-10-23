using Core.Application.Commands;
using Core.Application.DTOs.Domain;
using Core.Application.DTOs.Requests;
using Core.Application.DTOs.Responses;
using Core.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoutesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RoutesController> _logger;
    
    public RoutesController(IMediator mediator, ILogger<RoutesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Calculate optimal route between two nodes
    /// </summary>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(ApiResponse<RouteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<RouteDto>>> CalculateRoute([FromBody] CalculateRouteRequest request)
    {
        try
        {
            var preference = Enum.Parse<RoutingPreference>(request.Preference);
            
            var command = new CalculateRouteCommand(
                request.SourceNodeId,
                request.DestinationNodeId,
                preference,
                request.MaxHops);
            
            var route = await _mediator.Send(command);
            
            return Ok(ApiResponse<object?>.SuccessResult(route, "Route calculated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating route");
            return BadRequest(ApiResponse<RouteDto>.ErrorResult(ex.Message));
        }
    }
}