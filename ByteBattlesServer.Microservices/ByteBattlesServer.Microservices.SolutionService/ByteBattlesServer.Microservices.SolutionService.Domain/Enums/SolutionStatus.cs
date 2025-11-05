namespace ByteBattlesServer.Microservices.SolutionService.Domain.Enums;

public enum SolutionStatus
{
    Pending,
    Compiling,
    RunningTests,
    Completed,
    CompilationError,
    RuntimeError,
    TimeLimitExceeded,
    MemoryLimitExceeded,
    Failed
}