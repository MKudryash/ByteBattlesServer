using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using ByteBattlesServer.Microservices.SolutionService.Application.Queries;
using ByteBattlesServer.Microservices.SolutionService.Domain.Exceptions;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;
using ByteBattlesServer.Microservices.SolutionService.Domain.Models;
using MediatR;

namespace ByteBattlesServer.Microservices.SolutionService.Application.Handlers;

public class GetUserStatisticsQueryHandler : IRequestHandler<GetUserStatisticsQuery, SolutionStatisticsDto>
{
    private readonly ISolutionRepository _solutionRepository;

    public GetUserStatisticsQueryHandler(ISolutionRepository solutionRepository)
    {
        _solutionRepository = solutionRepository;
    }
    public async Task<SolutionStatisticsDto> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
    {
        var statistics = await _solutionRepository.GetUserStatisticsAsync(
            request.UserId);
        if (statistics == null)
            throw new UserSolutionException(request.UserId.ToString());

        return MapToDto(statistics);
    }

    private SolutionStatisticsDto MapToDto(SolutionStatistics solution) => new SolutionStatisticsDto()
    {
        TotalSubmissions = solution.TotalSubmissions,
        SuccessfulSubmissions = solution.SuccessfulSubmissions,
        SuccessRate = solution.SuccessRate,
        AverageExecutionTime = solution.AverageExecutionTime,
        FavoriteLanguage = solution.FavoriteLanguage,
        
    };

}