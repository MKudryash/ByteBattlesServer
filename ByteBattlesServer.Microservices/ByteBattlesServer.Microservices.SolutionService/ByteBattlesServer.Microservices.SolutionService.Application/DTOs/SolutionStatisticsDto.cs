namespace ByteBattlesServer.Microservices.SolutionService.Application.DTOs;

public record SolutionStatisticsDto
{
    public int TotalSubmissions { get; init; }
    public int SuccessfulSubmissions { get; init; }
    public double SuccessRate { get; init; }
    public double AverageExecutionTime { get; init; }
    public string FavoriteLanguage { get; init; }
}