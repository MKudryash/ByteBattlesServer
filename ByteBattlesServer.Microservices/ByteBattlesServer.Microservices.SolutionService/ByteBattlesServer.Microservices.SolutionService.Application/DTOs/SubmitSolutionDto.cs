using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.SolutionService.Application.DTOs;


public record SubmitSolutionDto(
    Guid TaskId,
    TaskDifficulty  Difficulty,
    Guid UserId,
    Guid LanguageId,
    string Code);