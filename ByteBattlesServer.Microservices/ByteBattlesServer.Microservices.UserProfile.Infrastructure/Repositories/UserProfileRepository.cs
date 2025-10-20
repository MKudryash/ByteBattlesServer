using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly UserProfileDbContext _context;

    public UserProfileRepository(UserProfileDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.UserProfile> GetByIdAsync(Guid id)
    {
        return await _context.UserProfiles
            .Include(up => up.Achievements)
            .ThenInclude(ua => ua.Achievement)
            .Include(up => up.BattleHistory)
            .FirstOrDefaultAsync(up => up.Id == id);
    }

    public async Task<Domain.Entities.UserProfile> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserProfiles
            .Include(up => up.Achievements)
            .ThenInclude(ua => ua.Achievement)
            .Include(up => up.BattleHistory)
            .FirstOrDefaultAsync(up => up.UserId == userId);
    }

    public async Task<List<Domain.Entities.UserProfile>> GetLeaderboardAsync(int topCount)
    {
        return await _context.UserProfiles
            .Where(up => up.IsPublic)
            .OrderByDescending(up => up.Stats.TotalExperience)
            .Take(topCount)
            .ToListAsync();
    }

    public async Task<List<Domain.Entities.UserProfile>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        return await _context.UserProfiles
            .Where(up => up.IsPublic && 
                (up.UserName.Contains(searchTerm) || up.Bio.Contains(searchTerm)))
            .OrderByDescending(up => up.Stats.TotalExperience)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Domain.Entities.UserProfile userProfile)
    {
        await _context.UserProfiles.AddAsync(userProfile);
    }

    public void Update(Domain.Entities.UserProfile userProfile)
    {
        _context.UserProfiles.Update(userProfile);
        
    }
}