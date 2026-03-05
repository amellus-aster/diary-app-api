using DiaryApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly DiaryDbContext _context;
    public AuthService(DiaryDbContext context)
    {
        _context = context;
    }
    public async Task<UserModel?> Authenticate(LoginRequest loginRequest)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
        if (user == null)
            return null;
        var hasher = new PasswordHasher<UserModel>();
        var result = hasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            loginRequest.Password
        );
        if (result == PasswordVerificationResult.Failed)
            return null;
        return user;
    }
    public async Task<UserModel?> SignUpAsync(SignUpResquest signUpResquest)
    {

        // 1. kiểm tra email tồn tại chưa
        if (await _context.Users.AnyAsync(u => u.Email == signUpResquest.Email))
        {
            return null;
        }
        var hasher = new PasswordHasher<UserModel>();
        var user = new UserModel
        {
            Email = signUpResquest.Email,
            CreatedAt = DateTime.UtcNow

        };
        user.PasswordHash = hasher.HashPassword(user, signUpResquest.Password);
        await _context.AddAsync(user);
        await _context.SaveChangesAsync();
        return user; 
    }
}