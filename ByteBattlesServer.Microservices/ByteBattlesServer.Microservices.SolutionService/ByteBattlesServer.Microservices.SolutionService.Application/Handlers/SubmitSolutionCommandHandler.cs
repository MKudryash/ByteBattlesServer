using ByteBattlesServer.Microservices.SolutionService.Application.Commands;
using ByteBattlesServer.Microservices.SolutionService.Application.DTOs;
using ByteBattlesServer.Microservices.SolutionService.Domain.Entities;
using ByteBattlesServer.Microservices.SolutionService.Domain.Enums;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using MediatR;

namespace ByteBattlesServer.Microservices.SolutionService.Application.Handlers;


public class SubmitSolutionCommandHandler : IRequestHandler<SubmitSolutionCommand, SolutionDto>
{
    private readonly ISolutionRepository _solutionRepository;
    private readonly ICompilationService _compilationService;
    private readonly ITaskServiceClient _taskServiceClient;
    private readonly IUserServiceClient _userServiceClient;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitSolutionCommandHandler(
        ISolutionRepository solutionRepository,
        ICompilationService compilationService,
        ITaskServiceClient taskServiceClient,
        IUserServiceClient userServiceClient,
        IUnitOfWork unitOfWork)
    {
        _solutionRepository = solutionRepository;
        _compilationService = compilationService;
        _taskServiceClient = taskServiceClient;
        _userServiceClient = userServiceClient;
        _unitOfWork = unitOfWork;
    }

    public async Task<SolutionDto> Handle(SubmitSolutionCommand request, CancellationToken cancellationToken)
    {
        // 1. Get task and test cases
        var task = await _taskServiceClient.GetTaskAsync(request.TaskId);
        var testCases = await _taskServiceClient.GetTestCasesAsync(request.TaskId);

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
            // 4. Compile code
             solution.UpdateStatus(SolutionStatus.Compiling, 0,testCases.Count);
             await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            var compilationResult = await _compilationService.CompileAsync(
                request.Code, request.LanguageId, task.FunctionName);
            
            if (!compilationResult.IsSuccess)
            {
                solution.UpdateStatus(SolutionStatus.CompilationError, 0, testCases.Count);
                attempt.UpdateStatus(SolutionStatus.CompilationError);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return MapToDto(solution);
            }
            
            // 5. Execute tests
            solution.UpdateStatus(SolutionStatus.RunningTests,0,testCases.Count);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            int passedTests = 0;
            var totalExecutionTime = TimeSpan.Zero;
            
            foreach (var testCaseDto in testCases)
            {
                var testResult = new TestResult(solution.Id, testCaseDto.Id, testCaseDto.Input, testCaseDto.ExpectedOutput);
                var testCase = new TestCase(testCaseDto.Id, testCaseDto.Input, testCaseDto.ExpectedOutput);
                var executionResult = await _compilationService.ExecuteTestAsync(
                    compilationResult.CompiledCode!, testCase, request.LanguageId);
            
                var testStatus = executionResult.IsSuccess && 
                               executionResult.Output == testCaseDto.ExpectedOutput
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

            // 6. Update solution status
            var averageExecutionTime = testCases.Count > 0 ? totalExecutionTime / testCases.Count : TimeSpan.Zero;
            var finalStatus = passedTests == testCases.Count ? SolutionStatus.Completed : SolutionStatus.Failed;
            
            solution.UpdateStatus(finalStatus, passedTests, testCases.Count, averageExecutionTime);
            attempt.UpdateStatus(finalStatus, averageExecutionTime);

            // 7. Update user stats
            await _userServiceClient.UpdateUserStatsAsync(
                request.UserId, 
                finalStatus == SolutionStatus.Completed,
                averageExecutionTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return MapToDto(solution);
        }
        catch (Exception ex)
        {
            solution.UpdateStatus(SolutionStatus.Failed, 0, testCases.Count);
            attempt.UpdateStatus(SolutionStatus.Failed);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw;
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