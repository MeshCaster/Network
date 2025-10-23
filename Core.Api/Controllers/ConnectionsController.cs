using Core.Application.Commands;
using Core.Application.DTOs.Domain;
using Core.Application.DTOs.Requests;
using Core.Application.DTOs.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConnectionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConnectionsController> _logger;
    
    public ConnectionsController(IMediator mediator, ILogger<ConnectionsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Create or update a connection between two nodes
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NodeConnectionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<NodeConnectionDto>>> CreateConnection([FromBody] CreateConnectionRequest request)
    {
        try
        {
            var command = new CreateNodeConnectionCommand(
                request.SourceNodeId,
                request.TargetNodeId,
                request.Quality,
                request.Latency,
                request.Throughput,
                request.PacketLoss,
                request.Rssi);
            
            var connection = await _mediator.Send(command);
            
            return Ok(ApiResponse<object?>.SuccessResult(connection, "Connection created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating connection");
            return BadRequest(ApiResponse<NodeConnectionDto>.ErrorResult(ex.Message));
        }
    }
}