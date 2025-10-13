using ByteBattlesServer.Microservices.AuthService.Domain.Entities;

namespace ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> GetByTokenAsync(string token);
    Task<RefreshToken> GetActiveByUserIdAsync(Guid userId);
    Task AddAsync(RefreshToken refreshToken);
    void Update(RefreshToken refreshToken);
    Task SaveChangesAsync();
}