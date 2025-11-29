using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.API.Messaging;

public class TaskInfoTaskMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<TaskInfoTaskMessageHandler> _logger; // Исправлено имя логгера

    public TaskInfoTaskMessageHandler(
        IServiceProvider serviceProvider,
        IMessageBus messageBus,
        ILogger<TaskInfoTaskMessageHandler> logger) // Исправлен параметр
    {
        _serviceProvider = serviceProvider;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task HandleTaskInfoRequest(TaskInfoRequest request)
    {
        _logger.LogInformation("🟠 [TaskServices] Received TaskInfoRequest for TaskId: {TaskId}", 
            request.TaskId); // Исправлено логирование

        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            var query = new GetTaskByIdQuery(request.TaskId);
            var task = await mediator.Send(query);

            if (task == null)
            {
                _logger.LogWarning("🔴 [TaskServices] Task not found for ID: {TaskId}", request.TaskId); // Исправлено сообщение
                var errorResponse = new TaskInfoResponse()
                {
                    Id = request.TaskId, // Исправлено свойство
                    CorrelationId = request.CorrelationId,
                    Success = false,
                    ErrorMessage = $"Not found task by id {request.TaskId}" // Исправлено сообщение
                };
                
                _messageBus.Publish(
                    errorResponse,
                    "codebattles.exchange",
                    "codebattles.info.response");
                return;
            }

            // Добавляем проверки на null для всех критических свойств
            if (task.TestCases == null)
            {
                _logger.LogWarning("⚠️ [TaskServices] TestCases is null for TaskId: {TaskId}", task.Id);
                task.TestCases = new List<TestCaseDto>();
            }

            if (task.Libraries == null)
            {
                _logger.LogWarning("⚠️ [TaskServices] Libraries is null for TaskId: {TaskId}", task.Id);
                task.Libraries = new List<LibraryDto>();
            }

            if (task.Language == null)
            {
                _logger.LogWarning("⚠️ [TaskServices] Language is null for TaskId: {TaskId}", task.Id);
                // Создаем язык по умолчанию или возвращаем ошибку
                var errorResponse = new TaskInfoResponse()
                {
                    Id = request.TaskId,
                    CorrelationId = request.CorrelationId,
                    Success = false,
                    ErrorMessage = $"Language not found for task {task.Id}"
                };
                
                _messageBus.Publish(
                    errorResponse,
                    "codebattles.exchange",
                    "codebattles.info.response");
                return;
            }

            _logger.LogInformation("🟢 [TaskServices] Task found: {TaskId}, TestCases count: {TestCasesCount}, Libraries count: {LibrariesCount}", 
                task.Id, task.TestCases.Count, task.Libraries.Count);

            // Безопасное создание response с проверками на null
            var response = new TaskInfoResponse()
            {
                Id = task.Id, // Исправлено свойство
                Title = task.Title ?? string.Empty,
                Description = task.Description ?? string.Empty,
                Author = task.Author ?? string.Empty,
                Difficulty = task.Difficulty,
                FunctionName = task.FunctionName ?? string.Empty,
                Parameters = task.Parameters ?? string.Empty,
                PatternFunction = task.PatternFunction ?? string.Empty,
                PatternMain = task.PatternMain ?? string.Empty,
                ReturnType = task.ReturnType ?? string.Empty,
                TestCases = task.TestCases?.Select(x => new TestCaseInfo()
                {
                    Input = x.Input ?? string.Empty,
                    Output = x.Output ?? string.Empty
                }).ToList() ?? new List<TestCaseInfo>(),
                Libraries = task.Libraries?.Select(x => new LibraryInfo()
                {
                    Id = x.Id,
                    NameLibrary = x.Name ?? string.Empty,
                    Description = x.Description ?? string.Empty
                }).ToList() ?? new List<LibraryInfo>(),
                Language = new LanguageInfo()
                {
                    Id = task.Language.Id,
                    Title = task.Language.Title ?? string.Empty,
                    ShortTitle = task.Language.ShortTitle ?? string.Empty,
                    FileExtension = task.Language.FileExtension ?? string.Empty,
                    CompilerCommand = task.Language.CompilerCommand ?? string.Empty,
                    ExecutionCommand = task.Language.ExecutionCommand ?? string.Empty,
                    SupportsCompilation = task.Language.SupportsCompilation,
                },
                CorrelationId = request.CorrelationId,
                Success = true
            };

            _logger.LogInformation("🟢 [TaskServices] Sending TaskInfoResponse for TaskId: {TaskId}", task.Id);

            _messageBus.Publish(
                response,
                "codebattles.exchange",
                "codebattles.info.response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [TaskServices] Error processing TaskInfoRequest for TaskId: {TaskId}", request.TaskId); // Исправлено

            var errorResponse = new TaskInfoResponse()
            {
                Id = request.TaskId, // Исправлено свойство
                CorrelationId = request.CorrelationId,
                Success = false,
                ErrorMessage = ex.Message
            };

            _messageBus.Publish(
                errorResponse, 
                "codebattles.exchange", 
                "codebattles.info.response");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🟠 [TaskServices] Starting TaskInfoTaskMessageHandler background service"); // Уточнено имя

        try
        {
            _messageBus.Subscribe<TaskInfoRequest>(
                "codebattles.exchange",
                "codebattles.task.requests", // Очередь для запросов задач
                "codebattles.info.request", // Routing key для запросов информации о задачах
                HandleTaskInfoRequest);

            _logger.LogInformation("🟢 [TaskServices] TaskInfoTaskMessageHandler subscriptions started successfully");
            
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("🟠 [TaskServices] TaskInfoTaskMessageHandler background service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [TaskServices] Error in TaskInfoTaskMessageHandler background service");
            throw;
        }
    }
}