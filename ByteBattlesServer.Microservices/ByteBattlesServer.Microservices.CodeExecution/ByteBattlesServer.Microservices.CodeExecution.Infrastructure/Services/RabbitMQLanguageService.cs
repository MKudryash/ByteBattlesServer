using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class RabbitMQLanguageService : ILanguageService
{
    private readonly IMessageBus _messageBus;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RabbitMQLanguageService> _logger;
    private readonly Dictionary<string, TaskCompletionSource<LanguageInfoResponse>> _pendingRequests = new();
    private readonly Dictionary<string, TaskCompletionSource<AllLanguagesResponse>> _pendingBatchRequests = new();
    private bool _isSubscribed = false;
    private readonly string _responseQueueName;
    private bool _disposed = false;


    private readonly object _lockObject = new object();

    public RabbitMQLanguageService(
        IMessageBus messageBus,
        IMemoryCache cache,
        ILogger<RabbitMQLanguageService> logger)
    {
        _messageBus = messageBus;
        _cache = cache;
        _logger = logger;
        
        // Создаем уникальное имя очереди для этого экземпляра сервиса
        _responseQueueName = $"language.testcases.responses.{Guid.NewGuid():N}";
        // Подписываемся на ответы при создании сервиса
        SubscribeToResponses();
    }

    public async Task<LanguageInfo> GetLanguageInfoAsync(Guid languageId)
    {
        var cacheKey = $"language_{languageId}";
        
        // Пробуем получить из кэша
        if (_cache.TryGetValue(cacheKey, out LanguageInfo cachedInfo))
        {
            return cachedInfo;
        }

        var request = new LanguageInfoRequest
        {
            LanguageId = languageId,
            ReplyToQueue = _responseQueueName,
            CorrelationId = Guid.NewGuid().ToString()
        };

        var tcs = new TaskCompletionSource<LanguageInfoResponse>();
        
        lock (_lockObject)
        {
            _pendingRequests[request.CorrelationId] = tcs;
        }

        try
        {
            // Убедимся, что подписка активна
            EnsureSubscribed();

            // Отправляем запрос
            _messageBus.Publish(
                request, 
                "language.exchange", 
                "language.info.request");

            _logger.LogDebug("Sent language info request for {LanguageId} with correlation {CorrelationId}", 
                languageId, request.CorrelationId);

            // Ждем ответ с таймаутом
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                _logger.LogWarning("Timeout waiting for language info response for {LanguageId}", languageId);
                throw new TimeoutException("Language info request timeout");
            }

            var response = await tcs.Task;

            if (!response.Success)
            {
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
            
            _logger.LogInformation("Successfully retrieved language info for {LanguageId}: {Title}", 
                languageId, languageInfo.Title);
            
            return languageInfo;
        }
        finally
        {
            lock (_lockObject)
            {
                _pendingRequests.Remove(request.CorrelationId);
            }
        }
    }

    public async Task<List<LanguageInfo>> GetAllLanguagesAsync()
    {
        var request = new AllLanguagesRequest
        {
            ReplyToQueue = "code_execution.language.responses",
            CorrelationId = Guid.NewGuid().ToString()
        };

        var tcs = new TaskCompletionSource<AllLanguagesResponse>();
        
        lock (_lockObject)
        {
            _pendingBatchRequests[request.CorrelationId] = tcs;
        }

        try
        {
            EnsureSubscribed();

            _messageBus.Publish(
                request,
                "language.exchange",
                "language.all.request");

            _logger.LogDebug("Sent all languages request with correlation {CorrelationId}", request.CorrelationId);

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                throw new TimeoutException("All languages request timeout");
            }

            var response = await tcs.Task;

            if (!response.Success)
            {
                throw new InvalidOperationException($"Failed to get all languages: {response.ErrorMessage}");
            }

            return response.Languages;
        }
        finally
        {
            lock (_lockObject)
            {
                _pendingBatchRequests.Remove(request.CorrelationId);
            }
        }
    }

    private void SubscribeToResponses()
    {
        if (_isSubscribed) return;

        try
        {
            // Подписываемся на ответы для отдельных языков
            _messageBus.Subscribe<LanguageInfoResponse>(
                "language.exchange",
                _responseQueueName,
                "language.info.response",
                async (response) =>
                {
                    _logger.LogDebug("Received language info response for correlation {CorrelationId}", 
                        response.CorrelationId);

                    TaskCompletionSource<LanguageInfoResponse> tcs;
                    lock (_lockObject)
                    {
                        _pendingRequests.TryGetValue(response.CorrelationId, out tcs);
                    }

                    if (tcs != null)
                    {
                        tcs.TrySetResult(response);
                    }
                    else
                    {
                        _logger.LogWarning("No pending request found for correlation {CorrelationId}", 
                            response.CorrelationId);
                    }
                });

            // Подписываемся на ответы для всех языков
            _messageBus.Subscribe<AllLanguagesResponse>(
                "language.exchange",
                "code_execution.language.responses", 
                "language.all.response",
                async (response) =>
                {
                    _logger.LogDebug("Received all languages response for correlation {CorrelationId}", 
                        response.CorrelationId);

                    TaskCompletionSource<AllLanguagesResponse> tcs;
                    lock (_lockObject)
                    {
                        _pendingBatchRequests.TryGetValue(response.CorrelationId, out tcs);
                    }

                    if (tcs != null)
                    {
                        tcs.TrySetResult(response);
                    }
                    else
                    {
                        _logger.LogWarning("No pending batch request found for correlation {CorrelationId}", 
                            response.CorrelationId);
                    }
                });

            _isSubscribed = true;
            _logger.LogInformation("Successfully subscribed to language responses");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to language responses");
        }
    }

    private void EnsureSubscribed()
    {
        if (!_isSubscribed)
        {
            lock (_lockObject)
            {
                if (!_isSubscribed)
                {
                    SubscribeToResponses();
                }
            }
        }
    }
}