namespace ByteBattlesServer.Microservices.SolutionService.Domain.Models;

public record TestCaseDto(Guid Id, string Input, string Output, bool IsHidden);