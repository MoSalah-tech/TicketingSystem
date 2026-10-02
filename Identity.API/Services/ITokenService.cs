using Identity.API.Domain;

namespace Identity.API.Services;

public interface ITokenService
{
    (string token, DateTime expiresAt) GenerateToken(User user);
}
