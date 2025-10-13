using ByteBattlesServer.Microservices.AuthService.Domain.Entities;
using ByteBattlesServer.Microservices.AuthService.Domain.Interfaces.Repositories;
using ByteBattlesServer.Microservices.AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByteBattlesServer.Microservices.AuthService.Infrastructure.Repositories;


public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<RefreshToken> GetActiveByUserIdAsync(Guid userId)
    {
        var now = DateTime.UtcNow;
        
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .Where(rt => rt.UserId == userId && 
                         rt.Revoked == null && 
                         rt.Expires > now)
            .OrderByDescending(rt => rt.Created)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
    }

    public void Update(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}