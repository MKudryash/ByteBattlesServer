using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using ByteBattlesServer.Microservices.SolutionService.Application.Queries;
using ByteBattlesServer.Microservices.SolutionService.Domain.Entities;
using ByteBattlesServer.Microservices.SolutionService.Domain.Exceptions;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;
using MediatR;

namespace ByteBattlesServer.Microservices.SolutionService.Application.Handlers;

public class GetTaskSolutionsQueryHandler : IRequestHandler<GetTaskSolutionsQuery, List<SolutionDto>>
{
    private readonly ISolutionRepository _solutionRepository;

    public GetTaskSolutionsQueryHandler(ISolutionRepository solutionRepository)
    {
        _solutionRepository = solutionRepository;
    }
    public async Task<List<SolutionDto>> Handle(GetTaskSolutionsQuery request, CancellationToken cancellationToken)
    {
        var solution = await _solutionRepository.GetByTaskAndUserAsync(request.TaskId,
            request.UserId);
        if (solution == null)
            throw new UserSolutionException(request.UserId.ToString());

        return solution.Select(x=> MapToDto(x)).ToList();
    }
    private SolutionDto MapToDto(Solution solution)
    {
        return new SolutionDto
        {
            Id = solution.Id,
            TaskId = solution.TaskId,
            UserId = solution.UserId,
            LanguageId = solution.LanguageId,
            Status = solution.Status.ToString(),
            SubmittedAt = solution.SubmittedAt,
            CompletedAt = solution.CompletedAt,
            ExecutionTime = solution.ExecutionTime,
            MemoryUsed = solution.MemoryUsed,
            PassedTests = solution.PassedTests,
            TotalTests = solution.TotalTests,
            SuccessRate = solution.SuccessRate,
            TestResults = solution.TestResults.Select(tr => new TestResultDto
            {
                Id = tr.Id,
                Status = tr.Status.ToString(),
                Input = tr.Input,
                ExpectedOutput = tr.ExpectedOutput,
                ActualOutput = tr.ActualOutput,
                ErrorMessage = tr.ErrorMessage,
                ExecutionTime = tr.ExecutionTime,
                MemoryUsed = tr.MemoryUsed
            }).ToList(),
            Attempts = solution.Attempts.Select(a => new SolutionAttemptDto
            {
                Id = a.Id,
                Code = a.Code,
                AttemptedAt = a.AttemptedAt,
                Status = a.Status.ToString(),
                ExecutionTime = a.ExecutionTime,
                MemoryUsed = a.MemoryUsed
            }).ToList()
        };
    }
}