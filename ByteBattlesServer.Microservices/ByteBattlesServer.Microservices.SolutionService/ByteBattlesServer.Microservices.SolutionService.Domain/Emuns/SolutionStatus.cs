namespace ByteBattlesServer.Microservices.SolutionService.Domain.Entities;

public enum SolutionStatus
{
    Pending,
    Processing,
    Success,
    Failed,
    Timeout
}