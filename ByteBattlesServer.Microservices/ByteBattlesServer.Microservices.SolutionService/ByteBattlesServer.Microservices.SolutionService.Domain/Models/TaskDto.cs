namespace ByteBattlesServer.Microservices.SolutionService.Domain.Models;

public record TaskDto(Guid Id, string Title, string Difficulty, string FunctionName);