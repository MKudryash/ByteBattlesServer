using System.Collections.Concurrent;
using ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ByteBattles.Microservices.CodeBattleServer.Infrastructure.Repositories;

public class RabbitMQTaskLanguageService : ITaskLanguageService, IDisposable
{
    private readonly IMessageBus _messageBus;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RabbitMQTaskLanguageService> _logger;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<TaskInfoResponse>> _pendingRequests = new();
    private readonly string _responseQueueName;
    private bool _disposed = false;

    public RabbitMQTaskLanguageService(
        IMessageBus messageBus,
        IMemoryCache cache,
        ILogger<RabbitMQTaskLanguageService> logger)
    {
        _messageBus = messageBus;
        _cache = cache;
        _logger = logger;
        
        // Создаем уникальную очередь для ответов
        _responseQueueName = $"codebattles.task.responses.{Guid.NewGuid():N}";
        
        SubscribeToResponses();
    }

    public async Task<TaskInfo> GetTaskInfoAsync(Guid languageId, Difficulty difficulty)
    {
        var cacheKey = $"task_{languageId}_{difficulty}";
        
        if (_cache.TryGetValue(cacheKey, out TaskInfo cachedInfo))
        {
            _logger.LogDebug("Returning cached task info for {LanguageId}", languageId);
            return cachedInfo;
        }

        var request = new TaskInfoRequest
        {
            Id = languageId,
            Difficulty = difficulty,
            ReplyToQueue = _responseQueueName,
            CorrelationId = Guid.NewGuid().ToString()
        };

        var tcs = new TaskCompletionSource<TaskInfoResponse>();
        _pendingRequests[request.CorrelationId] = tcs;

        try
        {
            _logger.LogInformation("🟠 [CodeExecution] Sending TaskInfoRequest for LanguageId: {LanguageId}, CorrelationId: {CorrelationId}", 
                languageId, request.CorrelationId);

            // Исправляем exchange и routing key
            _messageBus.Publish(
                request, 
                "codebattles.exchange", 
                "codebattles.info.request");

            // Уменьшим таймаут для тестирования
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                _logger.LogWarning("🔴 [CodeExecution] Timeout waiting for TaskInfoResponse for CorrelationId: {CorrelationId}", 
                    request.CorrelationId);
                throw new TimeoutException("Task info request timeout");
            }

            var response = await tcs.Task;

            if (!response.Success)
            {
                _logger.LogError("🔴 [CodeExecution] TaskInfoRequest failed: {ErrorMessage}", response.ErrorMessage);
                throw new InvalidOperationException($"Failed to get task info: {response.ErrorMessage}");
            }

            _logger.LogInformation("🟢 [CodeExecution] Received successful TaskInfoResponse for TaskId: {TaskId}", response.Id);

            var taskInfo = new TaskInfo
            {
                Id = response.Id,
                Title = response.Title,
                Description = response.Description,
                Author = response.Author,
                Difficulty = response.Difficulty,
                FunctionName = response.FunctionName,
                Parameters = response.Parameters,
                PatternFunction = response.PatternFunction,
                PatternMain = response.PatternMain,
                ReturnType = response.ReturnType,
                TestCases = response.TestCases,
                Libraries = response.Libraries,
            };

            // Кэшируем на 10 минут
            _cache.Set(cacheKey, taskInfo, TimeSpan.FromMinutes(10));
            
            return taskInfo;
        }
        finally
        {
            _pendingRequests.TryRemove(request.CorrelationId, out _);
        }
    }

    private void SubscribeToResponses()
    {
        try
        {
            _messageBus.Subscribe<TaskInfoResponse>(
                "codebattles.exchange",
                _responseQueueName,
                "codebattles.info.response",
                async (response) =>
                {
                    _logger.LogInformation("🟠 [CodeExecution] Received TaskInfoResponse for CorrelationId: {CorrelationId}, Success: {Success}", 
                        response.CorrelationId, response.Success);

                    if (_pendingRequests.TryGetValue(response.CorrelationId, out var tcs))
                    {
                        tcs.TrySetResult(response);
                    }
                    else
                    {
                        _logger.LogWarning("🟡 [CodeExecution] No pending request found for correlation {CorrelationId}", 
                            response.CorrelationId);
                    }
                });

            _logger.LogInformation("🟢 [CodeExecution] Successfully subscribed to task responses on queue: {QueueName}", _responseQueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [CodeExecution] Failed to subscribe to task responses");
            throw;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _pendingRequests.Clear();
            _disposed = true;
        }
    }
}