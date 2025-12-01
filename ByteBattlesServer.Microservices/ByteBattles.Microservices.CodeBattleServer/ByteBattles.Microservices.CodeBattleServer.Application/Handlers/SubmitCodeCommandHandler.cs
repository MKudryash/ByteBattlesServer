using ByteBattles.Microservices.CodeBattleServer.Application.Commands;
using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;
using ByteBattles.Microservices.CodeBattleServer.Domain.Exceptions;
using ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;
using ByteBattles.Microservices.CodeBattleServer.Domain.Services;
using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Handlers;

public class SubmitCodeCommandHandler : IRequestHandler<SubmitCodeCommand, SubmitCodeResponse>
{
    private readonly ICodeSubmissionRepository _codeSubmissionRepository;
    private readonly IBattleRoomRepository _battleRoomRepository;
    private readonly ICompilationService _compilationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SubmitCodeCommandHandler> _logger;

    public SubmitCodeCommandHandler(
        ICodeSubmissionRepository codeSubmissionRepository,
        IUnitOfWork unitOfWork, 
        ICompilationService compilationService, 
        IBattleRoomRepository battleRoomRepository,
        ILogger<SubmitCodeCommandHandler> logger)
    {
        _codeSubmissionRepository = codeSubmissionRepository;
        _compilationService = compilationService;
        _battleRoomRepository = battleRoomRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<SubmitCodeResponse> Handle(SubmitCodeCommand request, CancellationToken cancellationToken)
    {

        var room = await _battleRoomRepository.GetByIdAsync(request.RoomId);

        if (room == null)
        {
            _logger.LogWarning("🔴 [SubmitCode] Room not found: {RoomId}", request.RoomId);
            throw new BattleRoomNotFoundException(request.RoomId);
        }

        if (!room.Participants.Any(p => p.UserId == request.UserId))
        {
            _logger.LogWarning("🔴 [SubmitCode] User {UserId} not found in room {RoomId}", request.UserId, request.RoomId);
            throw new BattleRoomUserNotFoundException(request.UserId);
        }

        try
        {
            // Создаем submission
            var submission = new CodeSubmission(request.RoomId, request.UserId, request.Task.Id, request.Code);
            
            _logger.LogInformation("🟠 [SubmitCode] Executing tests for task: {TaskId}, Test cases: {TestCaseCount}", 
                request.Task.Id, request.Task.TestCases.Count());
            Console.WriteLine("Tests"+  request.Task.TestCases.ToList());
            // Выполняем тесты
            var executionResults = await _compilationService.ExecuteAllTestsAsync(
                request.Code, 
                request.Task.TestCases.ToList(), 
                request.Task.Language);
            
            _logger.LogInformation("🟢 [SubmitCode] Tests executed successfully. Results count: {ResultsCount}", 
                executionResults.Count());

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
            var finalStatus = passedTests == totalTests ? TestStatus.Passed : TestStatus.Failed;

            _logger.LogInformation("🟢 [SubmitCode] Test results: {PassedTests}/{TotalTests} passed, Status: {Status}", 
                passedTests, totalTests, finalStatus);

            
            // Создаем response
            var response = new SubmitCodeResponse
            {
                Id = submission.Id,
                TaskId = submission.TaskId,
                UserId = submission.UserId,
                LanguageId = request.Task.Language,
                Status = finalStatus,
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
                        Status = finalStatus.ToString(),
                        ExecutionTime = averageExecutionTime,
                        MemoryUsed = executionResults.FirstOrDefault()?.MemoryUsed ?? 0
                    }
                }
            };
            
            // Сохраняем в базу
            await _codeSubmissionRepository.AddAsync(submission);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("🟢 [SubmitCode] Code submission completed successfully. Submission ID: {SubmissionId}", 
                submission.Id);

            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [SubmitCode] Error during code submission for User: {UserId}, Room: {RoomId}", 
                request.UserId, request.RoomId);
            throw;
        }
    }
}