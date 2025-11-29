using ByteBattles.Microservices.CodeBattleServer.Application.Commands;
using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;
using ByteBattles.Microservices.CodeBattleServer.Domain.Exceptions;
using ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;
using ByteBattles.Microservices.CodeBattleServer.Domain.Models;
using ByteBattles.Microservices.CodeBattleServer.Domain.Services;
using ByteBattlesServer.Domain.enums;
using MediatR;

public class SubmitCodeCommandHandler : IRequestHandler<SubmitCodeCommand, SubmitCodeResponse>
{
    private readonly ICodeSubmissionRepository _codeSubmissionRepository;
    private readonly IBattleRoomRepository _battleRoomRepository;
    private readonly ICompilationService _compilationService;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitCodeCommandHandler(
        ICodeSubmissionRepository codeSubmissionRepository,
        IUnitOfWork unitOfWork, 
        ICompilationService compilationService, 
        IBattleRoomRepository battleRoomRepository)
    {
        _codeSubmissionRepository = codeSubmissionRepository;
        _compilationService = compilationService;
        _battleRoomRepository = battleRoomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubmitCodeResponse> Handle(SubmitCodeCommand request, CancellationToken cancellationToken)
    {
        var room = await _battleRoomRepository.GetByIdAsync(request.RoomId);

        Console.WriteLine("Sending code" + request.LanguageId);
        
        if (room == null)
            throw new BattleRoomNotFoundException(request.RoomId);

        if (!room.Participants.Any(p => p.UserId == request.UserId))
            throw new BattleRoomUserNotFoundException(request.UserId);

        // Создаем submission
        var submission = new CodeSubmission(request.RoomId, request.UserId, request.Task.Id, request.Code);
        
        // Выполняем тесты
        var executionResults = await _compilationService.ExecuteAllTestsAsync(
            request.Code, 
            request.Task.TestCases.ToList(), 
            request.LanguageId);
        
        int passedTests = 0;
        int totalTests = request.Task.TestCases.Count();
        var totalExecutionTime = TimeSpan.Zero;
        var testResults = new List<TestResultDto>();

        // Обрабатываем результаты тестов
        foreach (var (testCase, executionResult) in request.Task.TestCases.Zip(executionResults))
        {
            var testStatus = executionResult.IsSuccess && 
                           executionResult.Output?.Trim() == testCase.Output.Trim()
                ? TestStatus.Passed
                : TestStatus.Failed;

            var testResult = new TestResultDto
            {
                Id = Guid.NewGuid(),
                Status = testStatus.ToString(),
                Input = testCase.Input,
                ExpectedOutput = testCase.Output,
                ActualOutput = executionResult.Output,
                ErrorMessage = executionResult.ErrorMessage,
                ExecutionTime = executionResult.ExecutionTime,
                MemoryUsed = executionResult.MemoryUsed
            };

            testResults.Add(testResult);

            if (testStatus == TestStatus.Passed)
            {
                passedTests++;
            }

            totalExecutionTime += executionResult.ExecutionTime;
        }

      
        var averageExecutionTime = totalTests > 0 ? totalExecutionTime / totalTests : TimeSpan.Zero;
        var successRate = totalTests > 0 ? (double)passedTests / totalTests * 100 : 0;

    
        var response = new SubmitCodeResponse
        {
            Id = submission.Id,
            TaskId = submission.TaskId,
            UserId = submission.UserId,
            LanguageId = request.LanguageId,
            Status = passedTests == totalTests ? TestStatus.Passed : TestStatus.Failed,
            SubmittedAt = submission.SubmittedAt,
            CompletedAt = DateTime.UtcNow,
            ExecutionTime = averageExecutionTime,
            MemoryUsed = executionResults.FirstOrDefault()?.MemoryUsed ?? 0,
            PassedTests = passedTests,
            TotalTests = totalTests,
            SuccessRate = successRate,
            TestResults = testResults,
            Attempts = new List<SolutionAttemptDto> 
            {
                new SolutionAttemptDto
                {
                    Id = Guid.NewGuid(),
                    Code = request.Code,
                    AttemptedAt = submission.SubmittedAt,
                    Status = (passedTests == totalTests ? TestStatus.Passed : TestStatus.Failed).ToString(),
                    ExecutionTime = averageExecutionTime,
                    MemoryUsed = executionResults.FirstOrDefault()?.MemoryUsed ?? 0
                }
            }
        };
        
        await _codeSubmissionRepository.AddAsync(submission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return response;
    }
}