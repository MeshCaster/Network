using Core.Application.DTOs.Domain;
using Core.Application.DTOs.Responses;
using Core.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NetworkController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NetworkController> _logger;
    
    public NetworkController(IMediator mediator, ILogger<NetworkController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Get current network health status
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(ApiResponse<NetworkHealthDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NetworkHealthDto>>> GetHealth()
    {
        var health = await _mediator.Send(new GetNetworkHealthQuery());
        
        if (health == null)
            return Ok(ApiResponse<object>.ErrorResult("No health data available"));
        
        return Ok(ApiResponse<object>.SuccessResult(health));
    }
}