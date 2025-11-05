namespace ByteBattlesServer.Microservices.SolutionService.Domain.Models;

public record TestExecutionResult(bool IsSuccess, string? Output, string? ErrorMessage, TimeSpan ExecutionTime, int MemoryUsed);