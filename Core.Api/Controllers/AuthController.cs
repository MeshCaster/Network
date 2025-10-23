using Core.Application.DTOs.Requests;
using Core.Application.DTOs.Responses;
using Core.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }
    
    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(
                request.Email,
                request.Username,
                request.Password,
                request.FirstName,
                request.LastName,
                request.PhoneNumber);
            
            return Ok(ApiResponse<AuthResponse>.SuccessResult(response, "User registered successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return BadRequest(ApiResponse<AuthResponse>.ErrorResult(ex.Message));
        }
    }
    
    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request.EmailOrUsername, request.Password);
            
            return Ok(ApiResponse<AuthResponse>.SuccessResult(response, "Login successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return Unauthorized(ApiResponse<AuthResponse>.ErrorResult("Invalid credentials"));
        }
    }
}