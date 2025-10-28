namespace ByteBattlesServer.Microservices.SolutionService.Domain.Models;

public record TestCaseDto(Guid Id, string Input, string ExpectedOutput, bool IsHidden);