using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ByteBattlesServer.Microservices.TaskServices.API.Messaging;

public class LanguageMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<LanguageMessageHandler> _logger;

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
                SupportsCompilation = !string.IsNullOrEmpty(language.CompilerCommand),
                CorrelationId = request.CorrelationId,
                Success = true
            };

            // Отправляем ответ обратно
            _messageBus.Publish(
                response,
                "language.exchange",
                "language.info.response");

            _logger.LogDebug("Sent language info response for {LanguageId}", request.LanguageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling language info request for {LanguageId}", request.LanguageId);
            
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
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        try
        {
            var query = new SearchLanguagesQuery(null); // Get all languages
            var languages = await mediator.Send(query);
            
            var languageInfos = languages.Select(l => new LanguageInfo
            {
                Id = l.Id,
                Title = l.Title,
                ShortTitle = l.ShortTitle,
                FileExtension = l.FileExtension,
                CompilerCommand = l.CompilerCommand,
                ExecutionCommand = l.ExecutionCommand,
                SupportsCompilation = !string.IsNullOrEmpty(l.CompilerCommand)
            }).ToList();
            
            var response = new AllLanguagesResponse
            {
                Languages = languageInfos,
                CorrelationId = request.CorrelationId,
                Success = true
            };

            _messageBus.Publish(response, "language.exchange", "language.all.response");
            _logger.LogDebug("Sent all languages response with {Count} languages", languageInfos.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling all languages request");
            
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
        _logger.LogInformation("Starting LanguageMessageHandler background service");

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

        _logger.LogInformation("LanguageMessageHandler subscriptions started");
        
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("LanguageMessageHandler was cancelled");
        }
    }
}