using Microsoft.EntityFrameworkCore;
using Identity.API.Domain;
using Identity.API.Data;
using Microsoft.Identity.Client;

namespace Identity.API.Services;

public class AuthService : IAuthService
{
    private readonly IdentityDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthService(IdentityDbContext context, ITokenService tokenService)

    {
        _context = context;
        _tokenService = tokenService;

    }
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            throw new Exception("Email and Password are required .");

        if (dto.Password.Length < 6)
            throw new Exception("Password must be at least 6 characters");

        var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email.ToLower());
        if (exists) throw new Exception("Email already exists and registered.");

        var user = new User

        {

            Id = Guid.NewGuid(),
            Email = dto.Email.ToLowerInvariant(),
            FullName = dto.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "User",
            CreatedAt = DateTime.UtcNow


        };

        _context.Add(user);
        await _context.SaveChangesAsync();


        var (token, expiresAt) = _tokenService.GenerateToken(user);
        return new AuthResponseDto 
        { 
            Token = token, ExpiresAt = expiresAt 
        
        
        };
        

        


         





    }
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto) 
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

        if (user is null) throw new Exception("Invalid credentials.");
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid credentials.");

        var (token, expiresAt) = _tokenService.GenerateToken(user);
        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            Email = user.Email,
            Role = user.Role
        };

    }



}
