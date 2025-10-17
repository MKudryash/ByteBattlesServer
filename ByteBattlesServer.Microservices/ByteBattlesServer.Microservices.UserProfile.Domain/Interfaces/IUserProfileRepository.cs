namespace ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;

public interface IUserProfileRepository
{
    
    Task<Entities.UserProfile> GetByUserIdAsync(Guid userId);
    Task<List<Entities.UserProfile>> GetLeaderboardAsync(int topCount);
    Task<List<Entities.UserProfile>> SearchAsync(string searchTerm, int page, int pageSize);
    Task AddAsync(Entities.UserProfile userProfile);
    void Update(Entities.UserProfile userProfile);
}