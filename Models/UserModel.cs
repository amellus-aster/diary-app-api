namespace DiaryApi.Models;

public class UserModel
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}