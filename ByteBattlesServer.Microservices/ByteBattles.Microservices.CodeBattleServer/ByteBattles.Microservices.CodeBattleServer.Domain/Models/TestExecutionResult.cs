namespace ByteBattles.Microservices.CodeBattleServer.Domain.Models;

public record TestExecutionResult(bool IsSuccess, string? Output, string? ErrorMessage, TimeSpan ExecutionTime, int MemoryUsed);