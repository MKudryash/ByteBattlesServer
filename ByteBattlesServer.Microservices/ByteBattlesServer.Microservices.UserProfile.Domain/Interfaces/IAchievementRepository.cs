using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;

namespace ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;

public interface IAchievementRepository
{
    Task<Achievement> GetByIdAsync(Guid id);
    Task<List<Achievement>> GetAllAsync();
    Task<List<Achievement>> GetByTypeAsync(AchievementType type);
    Task AddAsync(Achievement achievement);
}