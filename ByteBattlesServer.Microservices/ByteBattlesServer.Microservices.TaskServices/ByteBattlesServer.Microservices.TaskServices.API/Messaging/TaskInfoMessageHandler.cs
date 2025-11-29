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
            
            var response = new TaskInfoResponse()
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Author = task.Author,
                Difficulty = task.Difficulty,
                FunctionName = task.FunctionName,
                Parameters = task.Parameters,
                PatternFunction = task.PatternFunction,
                PatternMain = task.PatternMain,
                ReturnType = task.ReturnType,
                TestCases = task.TestCases.Select(x => new TestCaseInfo()
                {
                    Input = x.Input,
                    Output = x.Output
                }).ToList(),
                Libraries = task.Libraries.Select(x => new LibraryInfo()
                {
                    Id = x.Id,
                    NameLibrary = x.Name,
                    Description = x.Description
                }).ToList(),
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
            
            // Ждем отмены вместо бесконечного ожидания
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