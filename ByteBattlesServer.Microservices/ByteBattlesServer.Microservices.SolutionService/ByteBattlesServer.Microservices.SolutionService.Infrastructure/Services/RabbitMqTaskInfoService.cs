using System.Collections.Concurrent;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;

public class RabbitMQTaskInfoService : ITaskInfoServices, IDisposable
{
    private readonly IMessageBus _messageBus;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RabbitMQTaskInfoService> _logger;
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<TaskInfoResponse>> _pendingRequests = new();
    private readonly string _responseQueueName;
    private bool _disposed = false;
    private bool _isSubscribed = false;
    private readonly object _subscribeLock = new object();

    public RabbitMQTaskInfoService(
        IMessageBus messageBus,
        IMemoryCache cache,
        ILogger<RabbitMQTaskInfoService> logger)
    {
        _messageBus = messageBus;
        _cache = cache;
        _logger = logger;
        
        // Updated queue name pattern
        _responseQueueName = $"solution.task.responses.{Guid.NewGuid():N}";
        
        SubscribeToResponses();
    }

    public async Task<TaskInfo> GetTaskInfoAsync(Guid taskId)
    {
        var cacheKey = $"task_info_{taskId}";
        
        // Try cache first
        if (_cache.TryGetValue(cacheKey, out TaskInfo cachedInfo))
        {
            _logger.LogDebug("🟣 [TaskInfoService] Returning cached task info for task {TaskId}", taskId);
            return cachedInfo;
        }

        var request = new TaskInfoRequest
        {
            TaskId = taskId,
            ReplyToQueue = _responseQueueName,
            CorrelationId = Guid.NewGuid()
        };

        var tcs = new TaskCompletionSource<TaskInfoResponse>();
        _pendingRequests[request.CorrelationId] = tcs;

        try
        {
            EnsureSubscribed();

            _logger.LogInformation("🟠 [TaskInfoService] Sending TaskInfoRequest for TaskId: {TaskId}, CorrelationId: {CorrelationId}", 
                taskId, request.CorrelationId);

            // Publish to solution exchange
            _messageBus.Publish(
                request, 
                "solution.tasks.exchange",  // Updated exchange
                "task.info.request");

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                _logger.LogWarning("🔴 [TaskInfoService] Timeout waiting for TaskInfoResponse for CorrelationId: {CorrelationId}", 
                    request.CorrelationId);
                throw new TimeoutException("Task info request timeout");
            }

            var response = await tcs.Task;

            if (!response.Success)
            {
                _logger.LogError("🔴 [TaskInfoService] TaskInfoRequest failed: {ErrorMessage}", response.ErrorMessage);
                throw new InvalidOperationException($"Failed to get task info: {response.ErrorMessage}");
            }

            _logger.LogInformation("🟢 [TaskInfoService] Received successful TaskInfoResponse for TaskId: {TaskId}", response.Id);

            var taskInfo = new TaskInfo
            {
                Id = response.Id,
                Title = response.Title ?? string.Empty,
                Description = response.Description ?? string.Empty,
                Author = response.Author ?? string.Empty,
                Difficulty = response.Difficulty,
                FunctionName = response.FunctionName ?? string.Empty,
                Parameters = response.Parameters ?? string.Empty,
                PatternFunction = response.PatternFunction ?? string.Empty,
                PatternMain = response.PatternMain ?? string.Empty,
                ReturnType = response.ReturnType ?? string.Empty,
                TestCases = response.TestCases ?? new List<TestCaseInfo>(),
                Libraries = response.Libraries ?? new List<LibraryInfo>(),
                Language = response.Language
            };

            // Cache for 10 minutes
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
        if (_isSubscribed) return;

        lock (_subscribeLock)
        {
            if (_isSubscribed) return;

            try
            {
                _logger.LogInformation("🟠 [TaskInfoService] Subscribing to task info responses on queue: {QueueName}", _responseQueueName);

                _messageBus.Subscribe<TaskInfoResponse>(
                    "solution.tasks.exchange",  // Updated exchange
                    _responseQueueName,
                    "task.info.response", 
                    async (response) =>
                    {
                        _logger.LogInformation("🟣 [TaskInfoService] Received task info response for correlation {CorrelationId}, Success: {Success}", 
                            response?.CorrelationId, response?.Success);

                        if (response == null)
                        {
                            _logger.LogWarning("⚠️ [TaskInfoService] Received null response");
                            return;
                        }

                        if (_pendingRequests.TryRemove(response.CorrelationId, out var tcs))
                        {
                            tcs.TrySetResult(response);
                            _logger.LogInformation("✅ [TaskInfoService] Successfully processed response for correlation {CorrelationId}", 
                                response.CorrelationId);
                        }
                        else
                        {
                            _logger.LogWarning("⚠️ [TaskInfoService] No pending request found for correlation {CorrelationId}", 
                                response.CorrelationId);
                        }
                    });

                _isSubscribed = true;
                _logger.LogInformation("🟢 [TaskInfoService] Successfully subscribed to task info responses");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔴 [TaskInfoService] Failed to subscribe to task info responses");
                throw;
            }
        }
    }

    private void EnsureSubscribed()
    {
        if (!_isSubscribed)
        {
            SubscribeToResponses();
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            foreach (var pendingRequest in _pendingRequests.Values)
            {
                pendingRequest.TrySetCanceled();
            }
            _pendingRequests.Clear();
            _disposed = true;
            _logger.LogInformation("🟠 [TaskInfoService] Disposed");
        }
    }
}