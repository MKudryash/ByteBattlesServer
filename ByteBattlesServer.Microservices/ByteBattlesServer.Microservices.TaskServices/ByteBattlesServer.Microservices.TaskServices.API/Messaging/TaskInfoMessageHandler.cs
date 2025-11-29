using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;

public class TaskInfoMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<TaskInfoMessageHandler> _logger;

    public TaskInfoMessageHandler(
        IServiceProvider serviceProvider,
        IMessageBus messageBus,
        ILogger<TaskInfoMessageHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task HandleTaskInfoRequest(TaskInfoRequest request)
    {
        _logger.LogInformation("🟠 [TaskServices] Received TaskInfoRequest for LanguageId: {LanguageId}, Difficulty: {Difficulty}", 
            request.Id, request.Difficulty);

        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            var query = new GetLanguageByIdQuery(request.Id);
            var language = await mediator.Send(query);

            if (language == null)
            {
                _logger.LogWarning("🔴 [TaskServices] Language not found for ID: {LanguageId}", request.Id);
                var errorResponse = new TaskInfoResponse()
                {
                    Id = request.Id,
                    CorrelationId = request.CorrelationId,
                    Success = false,
                    ErrorMessage = $"Not found language by {request.Id}"
                };
                
                _messageBus.Publish(
                    errorResponse,
                    "codebattles.exchange",
                    "codebattles.info.response");
                return;
            }

            var queryTask = new GetRandomTask(request.Difficulty);
            var task = await mediator.Send(queryTask);
            
            if (task == null)
            {
                _logger.LogWarning("🔴 [TaskServices] Task not found for difficulty: {Difficulty}", request.Difficulty);
                var errorResponse = new TaskInfoResponse()
                {
                    Id = request.Id,
                    CorrelationId = request.CorrelationId,
                    Success = false,
                    ErrorMessage = $"Not found task for difficulty {request.Difficulty}"
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
                task.TestCases = new List<TestCaseDto>(); // Инициализируем пустым списком
            }

            if (task.Libraries == null)
            {
                _logger.LogWarning("⚠️ [TaskServices] Libraries is null for TaskId: {TaskId}", task.Id);
                task.Libraries = new List<LibraryDto>(); // Инициализируем пустым списком
            }

            _logger.LogInformation("🟢 [TaskServices] Task found: {TaskId}, TestCases count: {TestCasesCount}, Libraries count: {LibrariesCount}", 
                task.Id, task.TestCases.Count, task.Libraries.Count);

            // Безопасное создание response с проверками на null
            var response = new TaskInfoResponse()
            {
                Id = task.Id,
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
                    Id = language.Id,
                    Title = language.Title ?? string.Empty,
                    ShortTitle = language.ShortTitle ?? string.Empty,
                    FileExtension = language.FileExtension ?? string.Empty,
                    CompilerCommand = language.CompilerCommand ?? string.Empty,
                    ExecutionCommand = language.ExecutionCommand ?? string.Empty,
                    SupportsCompilation = language.SupportsCompilation,
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
            _logger.LogError(ex, "🔴 [TaskServices] Error processing TaskInfoRequest for LanguageId: {LanguageId}", request.Id);

            var errorResponse = new TaskInfoResponse()
            {
                Id = request.Id,
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
        _logger.LogInformation("🟠 [TaskServices] Starting TaskInfo background service");

        try
        {
            _messageBus.Subscribe<TaskInfoRequest>(
                "codebattles.exchange",
                "codebattles.task.requests",
                "codebattles.info.request",
                HandleTaskInfoRequest);

            _logger.LogInformation("🟢 [TaskServices] TaskInfo subscriptions started successfully");
            
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("🟠 [TaskServices] TaskInfo background service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [TaskServices] Error in TaskInfo background service");
            throw;
        }
    }
}