namespace ByteBattles.Microservices.CodeBattleServer.Domain.Models;

public record TestCaseDto(Guid Id, string Input, string Output, bool IsHidden);