using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AirportBudget.Server.DTOs;
using AirportBudget.Server.Models;

namespace AirportBudget.Server.Services;

public class TokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _secretKey;
    private readonly string _signingAlgorithm;
    private readonly SymmetricSecurityKey _securityKey;

    public TokenService(IConfiguration configuration)
    {
        _issuer = configuration["JwtSettings:Issuer"]!;
        _audience = configuration["JwtSettings:Audience"]!;
        _secretKey = configuration["JwtSettings:SecretKey"]!;
        _signingAlgorithm = SecurityAlgorithms.HmacSha256;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    }

    public string GenerateJwtToken(User user)
    {
        var signingCredentials = new SigningCredentials(_securityKey, _signingAlgorithm);

        var claims = new List<Claim>
        {
            new Claim("UserID", user.UserId.ToString()) 
        };

        // 將用戶權限信息加入到 Claims 中
        //if (user.Group != null && user.Group.GroupPermissions != null)
        //{
        //    foreach (var permission in user.Group.GroupPermissions)
        //    {
        //        claims.Add(new Claim($"Permission_{permission.PermissionType}", $"{permission.CanGet},{permission.CanPost},{permission.CanPut},{permission.CanDelete}"));
        //    }
        //}

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Audience = _audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(2),
            SigningCredentials = signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public UserDTO GetUser(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst("UserID");

        var userId = userIdClaim?.Value ?? "0";

        return new UserDTO
        {
            UserId = int.Parse(userId),
        };
    }
}
