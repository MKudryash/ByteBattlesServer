using ByteBattlesServer.Microservices.SolutionService.Application.Commands;
using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using ByteBattlesServer.Microservices.SolutionService.Domain.Entities;
using ByteBattlesServer.Microservices.SolutionService.Domain.Enums;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.SolutionService.Domain.Models;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;

namespace ByteBattlesServer.Microservices.SolutionService.Application.Handlers;

public class SubmitSolutionCommandHandler : IRequestHandler<SubmitSolutionCommand, SolutionDto>
{
    private readonly ISolutionRepository _solutionRepository;
    private readonly ICompilationService _compilationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITestCasesServices _testCasesServices;
    private readonly IMessageBus _messageBus;

    public SubmitSolutionCommandHandler(
        ISolutionRepository solutionRepository,
        ICompilationService compilationService,
        IUnitOfWork unitOfWork,
        ITestCasesServices testCasesServices,
        IMessageBus messageBus)
    {
        _solutionRepository = solutionRepository;
        _compilationService = compilationService;
        _unitOfWork = unitOfWork;
        _testCasesServices = testCasesServices;
        _messageBus = messageBus;
    }

    public async Task<SolutionDto> Handle(SubmitSolutionCommand request, CancellationToken cancellationToken)
    {
        // 1. Get task and test cases
        //var task = await _taskServiceClient.GetTaskAsync(request.TaskId);
        //var testCases = await _taskServiceClient.GetTestCasesAsync(request.TaskId);

        var testCasesInfo = await _testCasesServices.GetTestCasesInfoAsync(request.TaskId);

        var testCases = testCasesInfo.Select(x => 
            new TestCaseDto(x.TaskId, x.Input, x.Output, false)).ToList();
        
        // 2. Create solution entity
        var solution = new Solution(request.TaskId, request.UserId, request.LanguageId, request.Code);
        await _solutionRepository.AddAsync(solution);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 3. Create attempt
        var attempt = new SolutionAttempt(solution.Id, request.Code);
        solution.AddAttempt(attempt);
        await _solutionRepository.AddAttemptAsync(attempt);

        try
        {
            // 4. Update status to Compiling
            solution.UpdateStatus(SolutionStatus.Compiling, 0, testCases.Count);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Execute all tests at once using batch execution
            solution.UpdateStatus(SolutionStatus.RunningTests, 0, testCases.Count);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // var executionResults = await _compilationService.ExecuteAllTestsAsync(
            //     request.Code, 
            //     testCases,
            //     request.LanguageId);

            var executionResults = await _compilationService.ExecuteAllTestsAsync(request.Code,
                testCases,
                request.LanguageId);

            int passedTests = 0;
            var totalExecutionTime = TimeSpan.Zero;

            // 6. Process test results
            foreach (var (testCaseDto, executionResult) in testCases.Zip(executionResults))
            {
                var testResult = new TestResult(solution.Id, testCaseDto.Id, testCaseDto.Input, testCaseDto.Output);
                
                var testStatus = executionResult.IsSuccess && 
                               executionResult.Output?.Trim() == testCaseDto.Output.Trim()
                    ? TestStatus.Passed
                    : TestStatus.Failed;

                testResult.UpdateResult(
                    testStatus,
                    executionResult.Output,
                    executionResult.ErrorMessage,
                    executionResult.ExecutionTime,
                    executionResult.MemoryUsed);

                solution.AddTestResult(testResult);
                await _solutionRepository.AddTestResultAsync(testResult);

                if (testStatus == TestStatus.Passed)
                {
                    passedTests++;
                }

                totalExecutionTime += executionResult.ExecutionTime;
            }

            // 7. Update solution status
            var averageExecutionTime = testCases.Count > 0 ? totalExecutionTime / testCases.Count : TimeSpan.Zero;
            var finalStatus = passedTests == testCases.Count ? SolutionStatus.Completed : SolutionStatus.Failed;
            
            solution.UpdateStatus(finalStatus, passedTests, testCases.Count, averageExecutionTime);
            attempt.UpdateStatus(finalStatus, averageExecutionTime);

            // 8. Update user stats
            var userUpdateStats = new UserStatsIntegrationEvent()
            {
                UserId = request.UserId,
                isSuccessful = finalStatus == SolutionStatus.Completed,
                difficulty = request.Difficulty,
                executionTime = averageExecutionTime,
                taskId = request.TaskId
            };
            _messageBus.Publish(
                userUpdateStats,
                "user_stats-events",
                "user.stats.update");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return MapToDto(solution);
        }
        catch (Exception ex)
        {
            // Handle compilation or execution errors
            solution.UpdateStatus(SolutionStatus.Failed, 0, testCases.Count);
            attempt.UpdateStatus(SolutionStatus.Failed);
            
            // Create error test results for all test cases
            foreach (var testCaseDto in testCases)
            {
                var testResult = new TestResult(solution.Id, testCaseDto.Id, testCaseDto.Input, testCaseDto.Output);
                testResult.UpdateResult(
                    TestStatus.Failed,
                    null,
                    $"Execution failed: {ex.Message}",
                    TimeSpan.Zero,
                    0);
                
                solution.AddTestResult(testResult);
                await _solutionRepository.AddTestResultAsync(testResult);
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            // Re-throw the exception to be handled by the exception middleware
            throw new ApplicationException($"Solution execution failed: {ex.Message}", ex);
        }
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