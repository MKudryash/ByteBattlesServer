namespace ByteBattlesServer.Microservices.SolutionService.Domain.Models;

public record CompilationResult(bool IsSuccess, string? CompiledCode, string? ErrorMessage, TimeSpan CompilationTime);