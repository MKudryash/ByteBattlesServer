using ByteBattlesServer.Microservices.SolutionService.Domain.Models;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;

public interface IUserServiceClient
{
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    Task UpdateUserStatsAsync(Guid userId, bool isSuccessful, TimeSpan executionTime);
}