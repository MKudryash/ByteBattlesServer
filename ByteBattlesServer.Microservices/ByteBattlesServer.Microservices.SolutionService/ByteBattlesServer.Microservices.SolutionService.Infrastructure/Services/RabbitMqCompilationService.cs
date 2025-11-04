using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.SolutionService.Domain.Models;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;


public class RabbitMqCompilationService : ICompilationService
{
    private readonly IMessageBus _messageBus;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RabbitMqCompilationService> _logger;
    private readonly Dictionary<string, TaskCompletionSource<CodeTestResultResponseEvent>> _pendingRequests = new();
    private bool _isSubscribed = false;
    private readonly object _lockObject = new object();
    public RabbitMqCompilationService(
        IMessageBus messageBus,
        IMemoryCache cache,
        ILogger<RabbitMqCompilationService> logger)
    {
        _messageBus = messageBus;
        _cache = cache;
        _logger = logger;
        
        // Подписываемся на ответы при создании сервиса
        SubscribeToResponses();
    }

    public async Task<List<TestExecutionResult>> ExecuteAllTestsAsync(string compiledCode,
        List<TestCaseDto> testCasesDto,
        Guid languageId)
    {


        var request = new CodeSubmissionEvent()
        {
            Code = compiledCode,
            Language = languageId,
            TestCases = testCasesDto.Select(x=> new TestCaseEvent()
            {
                Input = x.Input,
                Output = x.Output
            }).ToList(),
            ReplyToQueue = "solution.compiler.responses",
            CorrelationId = Guid.NewGuid().ToString()
        };

        var tcs = new TaskCompletionSource<CodeTestResultResponseEvent>();

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
                "code_execution.exchange",
                "compiler.info.request");


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
                throw new InvalidOperationException($"Failed to compiler info: {response.ErrorMessage}");
            }
            
            var results = new List<TestExecutionResult>();
   
            
            if (response?.Results != null)
            {
                foreach (var (testCase, resultDto) in testCasesDto.Zip(response.Results))
                {
                    results.Add(new TestExecutionResult(
                        resultDto.IsPassed,
                        resultDto.ActualOutput,
                        resultDto.IsPassed ? null : $"Expected: {resultDto.Output}, Actual: {resultDto.ActualOutput}",
                        resultDto.ExecutionTime,
                        0 // Memory usage not available
                    ));
                }
            }

            return  results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing batch tests for language {LanguageId}", languageId);
            throw;
        }
    }

    private void SubscribeToResponses()
    {
        if (_isSubscribed) return;

        try
        {
            _messageBus.Subscribe<CodeTestResultResponseEvent>(
                "compiler.exchange",
                "solution.compiler.responses", 
                "code_execution.compiler.response",
                async (response) =>
                {

                    TaskCompletionSource<CodeTestResultResponseEvent> tcs;
                    lock (_lockObject)
                    {
                        _pendingRequests.TryGetValue(response.CorrelationId, out tcs);
                    }

                    if (tcs != null)
                    {
                        tcs.TrySetResult(response);
                    }
                });

            _isSubscribed = true;
            _logger.LogInformation("Successfully subscribed to compiler responses");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to compiler responses");
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