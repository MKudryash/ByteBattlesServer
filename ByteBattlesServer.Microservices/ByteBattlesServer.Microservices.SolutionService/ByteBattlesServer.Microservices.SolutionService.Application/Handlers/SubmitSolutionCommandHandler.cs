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
    private readonly ITaskInfoServices _taskInfoServices;
    private readonly IMessageBus _messageBus;

    public SubmitSolutionCommandHandler(
        ISolutionRepository solutionRepository,
        ICompilationService compilationService,
        IUnitOfWork unitOfWork,
        ITaskInfoServices taskInfoServices,
        IMessageBus messageBus)
    {
        _solutionRepository = solutionRepository;
        _compilationService = compilationService;
        _unitOfWork = unitOfWork;
        _taskInfoServices = taskInfoServices;
        _messageBus = messageBus;
    }

    public async Task<SolutionDto> Handle(SubmitSolutionCommand request, CancellationToken cancellationToken)
    {

        var task = await _taskInfoServices.GetTaskInfoAsync(request.TaskId);

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
            solution.UpdateStatus(SolutionStatus.Compiling, 0, task.TestCases.Count);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Execute all tests at once using batch execution
            solution.UpdateStatus(SolutionStatus.RunningTests, 0, task.TestCases.Count);
            await _unitOfWork.SaveChangesAsync(cancellationToken);


            if (task.Language != null)
            {
                var testCasesForResult =  task.TestCases.Select(x => new TestCaseDto(x.Id, x.Input, x.Output, x.Hidden)).ToList();
                Console.WriteLine("Tests"+ testCasesForResult.Count);
                
                var executionResults = await _compilationService.ExecuteAllTestsAsync(
                    request.Code,
                    testCasesForResult,
                    task.Language,
                    task.Libraries.Select(l=> new LibraryInfo()
                    {
                        Description = l.Description,
                        NameLibrary = l.NameLibrary,
                        Id = l.Id
                    }).ToList(),
                    task.PatternMain);
                if (executionResults == null || !executionResults.Any())
                {
                   Console.WriteLine("❌ [Solution] No execution results received from compiler");
                    throw new InvalidOperationException("No test results received from compiler");
                }
    
                // ПРОВЕРЬТЕ КОЛИЧЕСТВО:
                Console.WriteLine($"📊 Received {executionResults.Count} execution results, expected {task.TestCases.Count}");
    
                // БЕЗОПАСНЫЙ ДОСТУП:
                var firstResult = executionResults.FirstOrDefault();
                if (firstResult != null)
                {
                    Console.WriteLine($"First result IsSuccess: {firstResult.IsSuccess}");
                }
                else
                {
                    Console.WriteLine("No execution results available");
                }

                Console.WriteLine(executionResults.FirstOrDefault().IsSuccess);
                int passedTests = 0;
                var totalExecutionTime = TimeSpan.Zero;

                var testCases = task.TestCases;
                // 6. Process test results
                foreach (var (testCaseDto, executionResult) in task.TestCases.Zip(executionResults))
                {
                    var isPassed = executionResult.IsSuccess &&
                                   executionResult.Output?.Trim() == testCaseDto.Output.Trim();
                    // Добавьте информативное логирование
                    Console.WriteLine($"Test {testCaseDto.Id}: " +
                                      $"Expected='{testCaseDto.Output}', " +
                                      $"Actual='{executionResult.Output}', " +
                                      $"IsSuccess={executionResult.IsSuccess}, " +
                                      $"Passed={isPassed}");
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
                    Console.WriteLine(testStatus);
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
                    IsSuccessful = finalStatus == SolutionStatus.Completed,
                    Difficulty = request.Difficulty,
                    ExecutionTime = averageExecutionTime,
                    TaskId = request.TaskId,
                    ProblemTitle = task.Title?? "Задача решена",
                    Language = task.Language.Title,
                    ActivityType = ActivityType.ProblemSolved
                };
                _messageBus.Publish(
                    userUpdateStats,
                    "user_stats-events",
                    "user.stats.update");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(solution);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Я УЛЕТЕЛ В ИСКЛЮЧЕНИЕ");
            // Handle compilation or execution errors
            solution.UpdateStatus(SolutionStatus.Failed, 0, task.TestCases.Count);
            attempt.UpdateStatus(SolutionStatus.Failed);

            // Create error test results for all test cases
            foreach (var testCaseDto in task.TestCases)
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