using System.Collections.Concurrent;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class RabbitMQLanguageService : ILanguageService, IDisposable
{
    private readonly IMessageBus _messageBus;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RabbitMQLanguageService> _logger;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<LanguageInfoResponse>> _pendingRequests = new();
    private readonly ConcurrentDictionary<string, TaskCompletionSource<AllLanguagesResponse>> _pendingBatchRequests = new();
    private readonly string _responseQueueName;
    private bool _disposed = false;

    public RabbitMQLanguageService(
        IMessageBus messageBus,
        IMemoryCache cache,
        ILogger<RabbitMQLanguageService> logger)
    {
        _messageBus = messageBus;
        _cache = cache;
        _logger = logger;
        
        // Создаем уникальную очередь для ответов
        _responseQueueName = $"code_execution.language.responses.{Guid.NewGuid():N}";
        
        SubscribeToResponses();
    }

    public async Task<LanguageInfo> GetLanguageInfoAsync(Guid languageId)
    {
        var cacheKey = $"language_{languageId}";
        
        if (_cache.TryGetValue(cacheKey, out LanguageInfo cachedInfo))
        {
            _logger.LogDebug("Returning cached language info for {LanguageId}", languageId);
            return cachedInfo;
        }

        var request = new LanguageInfoRequest
        {
            LanguageId = languageId,
            ReplyToQueue = _responseQueueName, // Используем уникальную очередь
            CorrelationId = Guid.NewGuid().ToString()
        };

        var tcs = new TaskCompletionSource<LanguageInfoResponse>();
        _pendingRequests[request.CorrelationId] = tcs;

        try
        {
            _logger.LogInformation("🟡 [CodeExecution] Sending language info request for {LanguageId}, correlation: {CorrelationId}, replyTo: {ReplyQueue}", 
                languageId, request.CorrelationId, _responseQueueName);

            _messageBus.Publish(
                request, 
                "language.exchange", 
                "language.info.request");

            // Уменьшим таймаут для тестирования
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                _logger.LogError("🔴 [CodeExecution] Timeout waiting for language info response for {LanguageId}", languageId);
                throw new TimeoutException("Language info request timeout");
            }

            var response = await tcs.Task;

            if (!response.Success)
            {
                _logger.LogError("🔴 [CodeExecution] Language info request failed for {LanguageId}: {ErrorMessage}", 
                    languageId, response.ErrorMessage);
                throw new InvalidOperationException($"Failed to get language info: {response.ErrorMessage}");
            }

            var languageInfo = new LanguageInfo
            {
                Id = response.LanguageId,
                Title = response.Title,
                ShortTitle = response.ShortTitle,
                FileExtension = response.FileExtension,
                CompilerCommand = response.CompilerCommand,
                ExecutionCommand = response.ExecutionCommand,
                SupportsCompilation = response.SupportsCompilation
            };

            // Кэшируем на 10 минут
            _cache.Set(cacheKey, languageInfo, TimeSpan.FromMinutes(10));
            
            _logger.LogInformation("🟢 [CodeExecution] Successfully retrieved language info for {LanguageId}: {Title}", 
                languageId, languageInfo.Title);
            
            return languageInfo;
        }
        finally
        {
            _pendingRequests.TryRemove(request.CorrelationId, out _);
        }
    }

    public async Task<List<LanguageInfo>> GetAllLanguagesAsync()
    {
        var request = new AllLanguagesRequest
        {
            ReplyToQueue = _responseQueueName, // Используем уникальную очередь
            CorrelationId = Guid.NewGuid().ToString()
        };

        var tcs = new TaskCompletionSource<AllLanguagesResponse>();
        _pendingBatchRequests[request.CorrelationId] = tcs;

        try
        {
            _logger.LogInformation("🟡 [CodeExecution] Sending all languages request, correlation: {CorrelationId}, replyTo: {ReplyQueue}", 
                request.CorrelationId, _responseQueueName);

            _messageBus.Publish(
                request,
                "language.exchange",
                "language.all.request");

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                _logger.LogError("🔴 [CodeExecution] Timeout waiting for all languages response");
                throw new TimeoutException("All languages request timeout");
            }

            var response = await tcs.Task;

            if (!response.Success)
            {
                _logger.LogError("🔴 [CodeExecution] All languages request failed: {ErrorMessage}", response.ErrorMessage);
                throw new InvalidOperationException($"Failed to get all languages: {response.ErrorMessage}");
            }

            _logger.LogInformation("🟢 [CodeExecution] Successfully retrieved {Count} languages", response.Languages.Count);
            return response.Languages;
        }
        finally
        {
            _pendingBatchRequests.TryRemove(request.CorrelationId, out _);
        }
    }

    private void SubscribeToResponses()
    {
        try
        {
            _logger.LogInformation("🟡 [CodeExecution] Subscribing to language responses on queue: {QueueName}", _responseQueueName);

            // Подписываемся на ответы для отдельных языков
            _messageBus.Subscribe<LanguageInfoResponse>(
                "language.exchange",
                _responseQueueName,
                "language.info.response",
                async (response) =>
                {
                    _logger.LogInformation("🟢 [CodeExecution] Received language info response for correlation {CorrelationId}", 
                        response.CorrelationId);

                    if (_pendingRequests.TryGetValue(response.CorrelationId, out var tcs))
                    {
                        tcs.TrySetResult(response);
                        _logger.LogInformation("🟢 [CodeExecution] Successfully processed language info response for {LanguageId}", 
                            response.LanguageId);
                    }
                    else
                    {
                        _logger.LogWarning("🟡 [CodeExecution] No pending request found for correlation {CorrelationId}", 
                            response.CorrelationId);
                    }
                });

            // Подписываемся на ответы для всех языков
            _messageBus.Subscribe<AllLanguagesResponse>(
                "language.exchange",
                _responseQueueName, 
                "language.all.response",
                async (response) =>
                {
                    _logger.LogInformation("🟢 [CodeExecution] Received all languages response for correlation {CorrelationId}, Count: {Count}", 
                        response.CorrelationId, response.Languages?.Count ?? 0);

                    if (_pendingBatchRequests.TryGetValue(response.CorrelationId, out var tcs))
                    {
                        tcs.TrySetResult(response);
                        _logger.LogInformation("🟢 [CodeExecution] Successfully processed all languages response");
                    }
                    else
                    {
                        _logger.LogWarning("🟡 [CodeExecution] No pending batch request found for correlation {CorrelationId}", 
                            response.CorrelationId);
                    }
                });

            _logger.LogInformation("🟢 [CodeExecution] Successfully subscribed to language responses");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [CodeExecution] Failed to subscribe to language responses");
            throw;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _pendingRequests.Clear();
            _pendingBatchRequests.Clear();
            _disposed = true;
        }
    }
}