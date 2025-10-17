using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Repositories;

public class AchievementRepository : IAchievementRepository
{
    private readonly UserProfileDbContext _context;

    public AchievementRepository(UserProfileDbContext context)
    {
        _context = context;
    }

    public async Task<Achievement> GetByIdAsync(Guid id)
    {
        return await _context.Achievements
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Achievement>> GetAllAsync()
    {
        return await _context.Achievements
            .OrderBy(a => a.Type)
            .ThenBy(a => a.RequiredValue)
            .ToListAsync();
    }

    public async Task<List<Achievement>> GetByTypeAsync(AchievementType type)
    {
        return await _context.Achievements
            .Where(a => a.Type == type)
            .OrderBy(a => a.RequiredValue)
            .ToListAsync();
    }

    public async Task AddAsync(Achievement achievement)
    {
        await _context.Achievements.AddAsync(achievement);
    }
}