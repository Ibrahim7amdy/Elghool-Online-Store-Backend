using Application.Interfaces.Security;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateCustomerToken(Customer customer)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, customer.Customer_Id.ToString()),
            new(JwtRegisteredClaimNames.Email, customer.Email),
            new("firstName", customer.FirstName),
            new("type", "customer")
        };

        return GenerateToken(claims);
    }

    public string GenerateAdminToken(Admin admin)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin.Admin_Id.ToString()),
            new(JwtRegisteredClaimNames.Email, admin.Email),
            new("firstName", admin.FirstName),
            new("type", "admin"),
            new("is_super_admin", admin.IsSuperAdmin.ToString().ToLower())
        };

        return GenerateToken(claims);
    }

    private string GenerateToken(List<Claim> claims)
    {
        var secretKey = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("JWT Key is missing.");
        }

        if (secretKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT Key must be at least 32 characters.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expireDaysStr = _configuration["Jwt:ExpireDays"]
                         ?? _configuration["JwtExpireDays"];

        if (!int.TryParse(expireDaysStr, out int expireDays))
        {
            expireDays = 7;
        }

        var issuer = _configuration["Jwt:Issuer"] ?? _configuration["JwtIssuer"];
        var audience = _configuration["Jwt:Audience"] ?? _configuration["JwtAudience"];

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(expireDays),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}