namespace ByteBattlesServer.Microservices.SolutionService.Domain.Models;

public record SolutionStatistics(int TotalSubmissions, int SuccessfulSubmissions, double SuccessRate, 
    double AverageExecutionTime, string FavoriteLanguage);