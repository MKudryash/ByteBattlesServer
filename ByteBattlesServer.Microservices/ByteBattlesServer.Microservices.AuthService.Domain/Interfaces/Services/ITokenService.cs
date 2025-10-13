using System.Security.Claims;
using ByteBattlesServer.Microservices.AuthService.Domain.Entities;

namespace ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}