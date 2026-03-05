using DiaryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DiaryApi.Controller;

using System.Security.Claims;

// ...existing code...
[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;
    private readonly DiaryDbContext _context;
    public AuthController(IAuthService authService, ITokenService tokenService, IConfiguration configuration, DiaryDbContext context)
    {
        _authService = authService;
        _tokenService = tokenService;
        _configuration = configuration;
        _context = context;
    }
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest loginRequest)
    {
        var user = await _authService.Authenticate(loginRequest);
        if (user == null) return Unauthorized();
        var accessToken = _tokenService.GenerateToken(user, _configuration);
        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();
        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        return Ok(response);
    }
    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register(SignUpResquest signUpResquest)
    {
        if (signUpResquest.Password != signUpResquest.ConfirmPassword)
        {
            return BadRequest("Passwords do not match");
        }
        var user = await _authService.SignUpAsync(signUpResquest);
        var accessToken = _tokenService.GenerateToken(user, _configuration);
        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();
        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
        return Ok(response);
    }
    [HttpPost("refresh-token")]
    public async Task<ActionResult<LoginResponse>> LoginWithRefreshToken(RefreshTokenRequest refreshTokenRequest)
    {
        //kiem tra co rt trong db khong
        //tra ve access token 
        var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenRequest.RefreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow) return Unauthorized();
        var newAccessToken = _tokenService.GenerateToken(user, _configuration);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = newAccessToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();
        var response = new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
        };
        return Ok(response);
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        var user = await _context.Users.FindAsync(int.Parse(userId));

        if (user == null)
            return NotFound();

        return Ok(new
        {
            user.Id,
            user.Email
        });
    }

}