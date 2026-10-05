using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Booking.API.Services;

public class ServiceTokenProvider : IServiceTokenProvider
{
    private readonly IConfiguration _configuration;
    public ServiceTokenProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string GetServiceToken()
    {
        var jwt = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,"system"),
            new Claim(ClaimTypes.NameIdentifier,"system"),
            new Claim(ClaimTypes.Name,"Booking Service Worker "),
            new Claim(ClaimTypes.Email,"worker@ticketing.internal"),
            new Claim(ClaimTypes.Email,"worker@ticketing.internal"),
            new Claim(ClaimTypes.Role, "Admin"),   // 👈 needed to call admin endpoints
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())






        };
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),   // short-lived
            signingCredentials: credentials

            );
        return new JwtSecurityTokenHandler().WriteToken(token); 

    }

}
