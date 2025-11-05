using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;

public class RabbitMQTestCasesService : ITestCasesServices
{
    private readonly IMessageBus _messageBus;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RabbitMQTestCasesService> _logger;
    private readonly Dictionary<string, TaskCompletionSource<TestCasesInfoResponse>> _pendingRequests = new();
    private bool _isSubscribed = false;
    private readonly object _lockObject = new object();

    public RabbitMQTestCasesService(
        IMessageBus messageBus,
        IMemoryCache cache,
        ILogger<RabbitMQTestCasesService> logger)
    {
        _messageBus = messageBus;
        _cache = cache;
        _logger = logger;
        
        // Подписываемся на ответы при создании сервиса
        SubscribeToResponses();
    }

    public async Task<List<TestCaseInfo>> GetTestCasesInfoAsync(Guid taskId)
    {
        var cacheKey = $"testcases_{taskId}";
        
        // Пробуем получить из кэша
        if (_cache.TryGetValue(cacheKey, out List<TestCaseInfo> cachedInfo))
        {
            return cachedInfo;
        }

        var request = new TestCaseInfoRequest()
        {
            TaskId = taskId,
            ReplyToQueue = "solution.testcase.responses",
            CorrelationId = Guid.NewGuid().ToString()
        };

        var tcs = new TaskCompletionSource<TestCasesInfoResponse>();
        
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
                "testcase.exchange",
                "testcase.info.request");
            

            // Ждем ответ с таймаутом
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                throw new TimeoutException("Task info request timeout");
            }

            var response = await tcs.Task;

            if (!response.Success)
            {
                throw new InvalidOperationException($"Failed to get testcases info: {response.ErrorMessage}");
            }

            var testCases = response.TestCases;

            // Кэшируем на 10 минут
            _cache.Set(cacheKey, testCases, TimeSpan.FromMinutes(10));

            
            return testCases;
        }
        finally
        {
            lock (_lockObject)
            {
                _pendingRequests.Remove(request.CorrelationId);
            }
        }
    }
    

    private void SubscribeToResponses()
    {
        if (_isSubscribed) return;

        try
        {
            _messageBus.Subscribe<TestCasesInfoResponse>(
                "testcase.exchange",
                "solution.testcase.responses",
                "testcase.info.response",
                async (response) =>
                {
                    _logger.LogDebug("Received all testcases response for correlation {CorrelationId}", 
                        response.CorrelationId);

                    TaskCompletionSource<TestCasesInfoResponse> tcs;
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
                        _logger.LogWarning("No pending batch request found for correlation {CorrelationId}", 
                            response.CorrelationId);
                    }
                });

            _isSubscribed = true;
            _logger.LogInformation("Successfully subscribed to testcases responses");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to testcases responses");
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