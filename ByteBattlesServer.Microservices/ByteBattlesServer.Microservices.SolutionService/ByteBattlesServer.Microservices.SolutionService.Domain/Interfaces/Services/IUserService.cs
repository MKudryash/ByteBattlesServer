namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;

public interface IUserService
{
    Task UpdateUserStatsAsync(Guid userId, bool isSuccess, TimeSpan executionTime);
}