using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.TaskServices.API.Messaging;

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
        _logger.LogInformation("🟠 [TaskServices] Received TaskInfoRequest for TaskId: {TaskId}", 
            request.TaskId);

        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            // Get task by ID
            var taskQuery = new GetTaskByIdQuery(request.TaskId);
            var task = await mediator.Send(taskQuery);

            if (task == null)
            {
                _logger.LogWarning("🔴 [TaskServices] Task not found for ID: {TaskId}", request.TaskId);
                var errorResponse = new TaskInfoResponse()
                {
                    Id = Guid.Empty,
                    CorrelationId = request.CorrelationId,
                    Success = false,
                    ErrorMessage = $"Task not found with id {request.TaskId}"
                };
                
                _messageBus.Publish(
                    errorResponse,
                    "solution.tasks.exchange",  // Match solution exchange
                    "task.info.response");
                return;
            }
            Console.WriteLine("TASKS TEST:" + task.TestCases.Count);
            // Get language
            var languageQuery = new GetLanguageByIdQuery(task.Language.Id);
            var language = await mediator.Send(languageQuery);

            if (language == null)
            {
                _logger.LogWarning("🔴 [TaskServices] Language not found for ID: {LanguageId}", task.Language.Id);
                var errorResponse = new TaskInfoResponse()
                {
                    Id = Guid.Empty,
                    CorrelationId = request.CorrelationId,
                    Success = false,
                    ErrorMessage = $"Language not found with id {task.Language.Id}"
                };
                
                _messageBus.Publish(
                    errorResponse,
                    "solution.tasks.exchange",
                    "task.info.response");
                return;
            }

            // Create response
            var response = CreateTaskInfoResponse(task, language, request.CorrelationId);
            
            _logger.LogInformation("🟢 [TaskServices] Sending TaskInfoResponse for TaskId: {TaskId}", task.Id);

            _messageBus.Publish(
                response,
                "solution.tasks.exchange",  // Match solution exchange
                "task.info.response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [TaskServices] Error processing TaskInfoRequest for TaskId: {TaskId}", request.TaskId);

            var errorResponse = new TaskInfoResponse()
            {
                Id = Guid.Empty,
                CorrelationId = request.CorrelationId,
                Success = false,
                ErrorMessage = ex.Message
            };

            _messageBus.Publish(
                errorResponse, 
                "solution.tasks.exchange",
                "task.info.response");
        }
    }

    private TaskInfoResponse CreateTaskInfoResponse(TaskDto task, LanguageDto language, Guid correlationId)
    {
        // Initialize collections if null
        task.TestCases ??= new List<TestCaseDto>();
        task.Libraries ??= new List<LibraryDto>();

        _logger.LogInformation("🟢 [TaskServices] Task found: {TaskId}, TestCases: {TestCasesCount}, Libraries: {LibrariesCount}", 
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
                Output = x.Output ?? string.Empty
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
        _logger.LogInformation("🟠 [TaskServices] Starting TaskInfoMessageHandler background service");

        try
        {
            _messageBus.Subscribe<TaskInfoRequest>(
                "solution.tasks.exchange",  // Subscribe to solution exchange
                "solution.task.requests.queue",
                "task.info.request",
                HandleTaskInfoRequest);

            _logger.LogInformation("🟢 [TaskServices] TaskInfoMessageHandler subscriptions started successfully");
            
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("🟠 [TaskServices] TaskInfoMessageHandler background service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [TaskServices] Error in TaskInfoMessageHandler background service");
            throw;
        }
    }
}