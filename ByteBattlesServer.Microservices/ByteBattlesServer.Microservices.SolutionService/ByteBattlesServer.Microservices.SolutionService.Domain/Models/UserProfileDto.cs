using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Models;

public record UserProfileDto(Guid UserId, string UserName, UserStatsDto Stats);