using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.API.Messaging;

public class RandomTaskMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<RandomTaskMessageHandler> _logger;

    public RandomTaskMessageHandler(
        IServiceProvider serviceProvider,
        IMessageBus messageBus,
        ILogger<RandomTaskMessageHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _messageBus = messageBus;
        _logger = logger;
    }

private async Task HandleRandomTaskRequest(TaskInfoRequest request)
{
    _logger.LogInformation("🟠 [TaskServices] Received RandomTaskRequest for LanguageId: {LanguageId}, Difficulty: {Difficulty}", 
        request.LanguageId, request.Difficulty);

    using var scope = _serviceProvider.CreateScope();
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

    try
    {
        // Get language first
        var languageQuery = new GetLanguageByIdQuery(request.LanguageId);
        var language = await mediator.Send(languageQuery);

        if (language == null)
        {
            _logger.LogWarning("🔴 [TaskServices] Language not found for ID: {LanguageId}", request.LanguageId);
            var errorResponse = new TaskInfoResponse()
            {
                Id = Guid.Empty,
                CorrelationId = request.CorrelationId,
                Success = false,
                ErrorMessage = $"Language not found with id {request.LanguageId}"
            };
            
            // FIX: Use the same exchange and correct routing key
            _messageBus.Publish(
                errorResponse,
                "codebattles.random.exchange",  // Same exchange as sender
                "task.random.response");        // Correct routing key for responses
            return;
        }

        // Get random task
        var taskQuery = new GetRandomTask(request.Difficulty, request.LanguageId);
        var task = await mediator.Send(taskQuery);
    
        
        
        if (task == null)
        {
            _logger.LogWarning("🔴 [TaskServices] Task not found for difficulty: {Difficulty} and language: {LanguageId}", 
                request.Difficulty, request.LanguageId);
            var errorResponse = new TaskInfoResponse()
            {
                Id = Guid.Empty,
                CorrelationId = request.CorrelationId,
                Success = false,
                ErrorMessage = $"No tasks found for difficulty {request.Difficulty} and language {request.LanguageId}"
            };
            
            // FIX: Use the same exchange and correct routing key
            _messageBus.Publish(
                errorResponse,
                "codebattles.random.exchange",  // Same exchange as sender
                "task.random.response");        // Correct routing key for responses
            return;
        }
        
        Console.WriteLine("TASKS TEST:" + task.TestCases.Count);
        // Safe mapping with null checks
        var response = CreateTaskInfoResponse(task, language, request.CorrelationId);
        
        _logger.LogInformation("🟢 [TaskServices] Sending RandomTaskResponse for TaskId: {TaskId}", task.Id);

        // FIX: Use the same exchange and correct routing key
        _messageBus.Publish(
            response,
            "codebattles.random.exchange",  // Same exchange as sender
            "task.random.response");        // Correct routing key for responses
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "🔴 [TaskServices] Error processing RandomTaskRequest for LanguageId: {LanguageId}", request.LanguageId);

        var errorResponse = new TaskInfoResponse()
        {
            Id = Guid.Empty,
            CorrelationId = request.CorrelationId,
            Success = false,
            ErrorMessage = ex.Message
        };

        // FIX: Use the same exchange and correct routing key
        _messageBus.Publish(
            errorResponse, 
            "codebattles.random.exchange",  // Same exchange as sender
            "task.random.response");        // Correct routing key for responses
    }
}

    private TaskInfoResponse CreateTaskInfoResponse(TaskDto task, LanguageDto language, Guid correlationId)
    {
        
        
        // Initialize collections if null
        task.TestCases ??= new List<TestCaseDto>();
        task.Libraries ??= new List<LibraryDto>();

        _logger.LogInformation("🟢 [TaskServices] Random task found: {TaskId}, TestCases count: {TestCasesCount}, Libraries count: {LibrariesCount}", 
            task.Id, task.TestCases.Count, task.Libraries.Count);

        return new TaskInfoResponse()
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
            TestCases = task.TestCases.Select(x => new TestCaseInfo()
            {
                Input = x.Input ?? string.Empty,
                Output = x.Output ?? string.Empty,
                Hidden = x.IsExample
            }).ToList(),
            Libraries = task.Libraries.Select(x => new LibraryInfo()
            {
                Id = x.Id,
                NameLibrary = x.Name ?? string.Empty,
                Description = x.Description ?? string.Empty
            }).ToList(),
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
            CorrelationId = correlationId,
            Success = true
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🟠 [TaskServices] Starting RandomTaskMessageHandler background service");

        try
        {
            _messageBus.Subscribe<TaskInfoRequest>(
                "codebattles.random.exchange",  // Отдельный exchange для случайных задач
                "codebattles.random.requests.queue",
                "task.random.request",  // Специфичный routing key
                HandleRandomTaskRequest);

            _logger.LogInformation("🟢 [TaskServices] RandomTaskMessageHandler subscriptions started successfully");
            
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("🟠 [TaskServices] RandomTaskMessageHandler background service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [TaskServices] Error in RandomTaskMessageHandler background service");
            throw;
        }
    }
}