
using DiaryApi.Models;

public interface ITokenService
{
    string GenerateToken(UserModel user, IConfiguration configuration);
    string GenerateRefreshToken();
}