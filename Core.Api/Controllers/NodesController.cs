using Core.Application.Commands;
using Core.Application.DTOs.Domain;
using Core.Application.DTOs.Requests;
using Core.Application.DTOs.Responses;
using Core.Application.Queries;
using Core.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NodesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NodesController> _logger;
    
    public NodesController(IMediator mediator, ILogger<NodesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Get all nodes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NodeDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NodeDto>>>> GetAll()
    {
        var nodes = await _mediator.Send(new GetAllNodesQuery());
        return Ok(ApiResponse<IEnumerable<NodeDto>>.SuccessResult(nodes));
    }
    
    /// <summary>
    /// Get node by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<NodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetById(Guid id)
    {
        var node = await _mediator.Send(new GetNodeByIdQuery(id));
        
        if (node == null)
            return NotFound(ApiResponse<NodeDto>.ErrorResult("Node not found"));
        
        return Ok(ApiResponse<object>.SuccessResult(node));
    }
    
    /// <summary>
    /// Register a new node
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NodeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<NodeDto>>> Register([FromBody] RegisterNodeRequest request)
    {
        try
        {
            var command = new RegisterNodeCommand(
                request.Name,
                Enum.Parse<NodeType>(request.Type),
                request.Latitude,
                request.Longitude,
                request.IpAddress,
                request.MacAddress,
                request.BandwidthCapacity);
            
            var node = await _mediator.Send(command);
            
            return CreatedAtAction(
                nameof(GetById), 
                new { id = Guid.NewGuid() }, 
                ApiResponse<object?>.SuccessResult(node, "Node registered successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering node");
            return BadRequest(ApiResponse<NodeDto>.ErrorResult(ex.Message));
        }
    }
    
    /// <summary>
    /// Update node heartbeat
    /// </summary>
    [HttpPost("{id}/heartbeat")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHeartbeat(Guid id, [FromBody] UpdateNodeStatusRequest request)
    {
        try
        {
            var command = new UpdateNodeHeartbeatCommand(
                id,
                request.SignalStrength,
                request.CpuUsage,
                request.MemoryUsage,
                request.Temperature);
            
            await _mediator.Send(command);
            
            return Ok(new { message = "Heartbeat updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating heartbeat for node {NodeId}", id);
            return NotFound(new { message = ex.Message });
        }
    }
}
