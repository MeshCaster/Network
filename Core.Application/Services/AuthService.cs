// MeshNetwork.Infrastructure/Services/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Core.Application.DTOs.Domain;
using Core.Application.DTOs.Responses;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Core.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Core.Application.Services;

public class AuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    
    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _mapper = mapper;
    }
    
    public async Task<AuthResponse> RegisterAsync(
        string email,
        string username,
        string password,
        string? firstName,
        string? lastName,
        string? phoneNumber)
    {
        // Check if email already exists
        if (await _unitOfWork.Users.EmailExistsAsync(email))
            throw new InvalidOperationException("Email already exists");
        
        // Check if username already exists
        if (await _unitOfWork.Users.UsernameExistsAsync(username))
            throw new InvalidOperationException("Username already exists");
        
        // Hash password
        var passwordHash = HashPassword(password);
        
        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Role = UserRole.User,
            IsActive = true,
            EmailConfirmed = false,
            IsRelayEnabled = false,
            RelayCredits = 0,
            TotalBytesRelayed = 0,
            CreatedAt = DateTime.UtcNow
        };
        
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        
        // Generate JWT token
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60"));
        
        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = _mapper.Map<UserDto>(user)
        };
    }
    
    public async Task<AuthResponse> LoginAsync(string emailOrUsername, string password)
    {
        // Find user by email or username
        var user = await _unitOfWork.Users.GetByEmailAsync(emailOrUsername);
        user ??= await _unitOfWork.Users.GetByUsernameAsync(emailOrUsername);
        
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");
        
        // Verify password
        if (!VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");
        
        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is deactivated");
        
        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        
        // Generate JWT token
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(
            int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60"));
        
        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = _mapper.Map<UserDto>(user)
        };
    }
    
    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey not configured");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");
        
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
    
    private bool VerifyPassword(string password, string hash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == hash;
    }
}