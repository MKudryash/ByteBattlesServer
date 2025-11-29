using System.Collections.Concurrent;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;

public class LanguageMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<LanguageMessageHandler> _logger;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<TaskInfoResponse>> _pendingRequests = new();
    public LanguageMessageHandler(
        IServiceProvider serviceProvider,
        IMessageBus messageBus,
        ILogger<LanguageMessageHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task HandleLanguageInfoRequest(LanguageInfoRequest request)
    {
        _logger.LogInformation(
            "🟠 [TaskServices] Received language info request for {LanguageId}, correlation: {CorrelationId}, replyTo: {ReplyToQueue}",
            request.LanguageId, request.CorrelationId, request.ReplyToQueue);

        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            var query = new GetLanguageByIdQuery(request.LanguageId);
            var language = await mediator.Send(query);

            var response = new LanguageInfoResponse
            {
                LanguageId = language.Id,
                Title = language.Title,
                ShortTitle = language.ShortTitle,
                FileExtension = language.FileExtension,
                CompilerCommand = language.CompilerCommand,
                ExecutionCommand = language.ExecutionCommand,
                SupportsCompilation = language.SupportsCompilation,
                CorrelationId = request.CorrelationId,
                Success = true
            };

            // ВАЖНО: Всегда используем фиксированный routing key для ответов
            // Не используем request.ReplyToQueue как routing key!
            var routingKey = "language.info.response";

            _logger.LogInformation(
                "🟠 [TaskServices] Sending language info response for {LanguageId} with routing: {RoutingKey}",
                request.LanguageId, routingKey);

            _messageBus.Publish(
                response,
                "language.exchange",
                routingKey);

            _logger.LogInformation("🟠 [TaskServices] Language info response sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [TaskServices] Error handling language info request for {LanguageId}",
                request.LanguageId);

            var errorResponse = new LanguageInfoResponse
            {
                LanguageId = request.LanguageId,
                CorrelationId = request.CorrelationId,
                Success = false,
                ErrorMessage = ex.Message
            };

            _messageBus.Publish(errorResponse, "language.exchange", "language.info.response");
        }
    }

    public async Task HandleAllLanguagesRequest(AllLanguagesRequest request)
    {
        _logger.LogInformation(
            "🟠 [TaskServices] Received all languages request, correlation: {CorrelationId}, replyTo: {ReplyToQueue}",
            request.CorrelationId, request.ReplyToQueue);

        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            var query = new SearchLanguagesQuery(null);
            var languages = await mediator.Send(query);

            var languageInfos = languages.Select(l => new LanguageInfo
            {
                Id = l.Id,
                Title = l.Title,
                ShortTitle = l.ShortTitle,
                FileExtension = l.FileExtension,
                CompilerCommand = l.CompilerCommand,
                ExecutionCommand = l.ExecutionCommand,
                SupportsCompilation = l.SupportsCompilation
            }).ToList();

            var response = new AllLanguagesResponse
            {
                Languages = languageInfos,
                CorrelationId = request.CorrelationId,
                Success = true
            };

            // ВАЖНО: Фиксированный routing key для ответов
            var routingKey = "language.all.response";

            _logger.LogInformation(
                "🟠 [TaskServices] Sending all languages response with {Count} languages with routing: {RoutingKey}",
                languageInfos.Count, routingKey);

            _messageBus.Publish(response, "language.exchange", routingKey);
            _logger.LogInformation("🟠 [TaskServices] All languages response sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [TaskServices] Error handling all languages request");

            var errorResponse = new AllLanguagesResponse
            {
                CorrelationId = request.CorrelationId,
                Success = false,
                ErrorMessage = ex.Message
            };

            _messageBus.Publish(errorResponse, "language.exchange", "language.all.response");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🟠 [TaskServices] Starting LanguageMessageHandler background service");

        try
        {
            // Подписываемся на оба типа запросов
            _messageBus.Subscribe<LanguageInfoRequest>(
                "language.exchange",
                "task_services.language.requests",
                "language.info.request",
                HandleLanguageInfoRequest);

            _messageBus.Subscribe<AllLanguagesRequest>(
                "language.exchange", 
                "task_services.language.requests",
                "language.all.request",
                HandleAllLanguagesRequest);

            _logger.LogInformation("🟢 [TaskServices] LanguageMessageHandler subscriptions started successfully");
            
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [TaskServices] Error in LanguageMessageHandler");
            throw;
        }
    }
}