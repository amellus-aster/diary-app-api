using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiaryApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
public class TokenService : ITokenService
{
    public string GenerateToken(UserModel user, IConfiguration configuration)
    {
        var jwt = configuration.GetSection("Jwt");
        var claims = new[]
        {
             new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString() ),
             new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };
        var keyBytes = Encoding.UTF8.GetBytes(jwt["SecretKey"]);
        var key = new SymmetricSecurityKey(keyBytes);
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"]));
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            signingCredentials: cred,
            expires: expiration
        );
        var tokenstring = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenstring;
    }
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}