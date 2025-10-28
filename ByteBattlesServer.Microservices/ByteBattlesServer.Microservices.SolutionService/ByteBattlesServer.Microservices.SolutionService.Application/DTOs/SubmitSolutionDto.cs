namespace ByteBattlesServer.Microservices.SolutionService.Application.DTOs;


public record SubmitSolutionDto(
    Guid TaskId,
    Guid UserId,
    Guid LanguageId,
    string Code);