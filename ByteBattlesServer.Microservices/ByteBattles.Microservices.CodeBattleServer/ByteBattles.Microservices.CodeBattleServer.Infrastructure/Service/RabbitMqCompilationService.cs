using System.Collections.Concurrent;
using ByteBattles.Microservices.CodeBattleServer.Domain.Models;
using ByteBattles.Microservices.CodeBattleServer.Domain.Services;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

public class RabbitMqCompilationService : ICompilationService, IDisposable
{
    private readonly IMessageBus _messageBus;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RabbitMqCompilationService> _logger;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<CodeTestResultResponseEvent>> _pendingRequests = new();
    private readonly string _responseQueueName;
    private bool _disposed = false;
    private bool _isSubscribed = false;

    public RabbitMqCompilationService(
        IMessageBus messageBus,
        IMemoryCache cache,
        ILogger<RabbitMqCompilationService> logger)
    {
        _messageBus = messageBus;
        _cache = cache;
        _logger = logger;
        
        // Создаем уникальное имя очереди для этого экземпляра сервиса
        _responseQueueName = $"solution.compiler.responses.{Guid.NewGuid():N}";
        
        SubscribeToResponses();
    }

    private void SubscribeToResponses()
    {
        try
        {
           
            _messageBus.Subscribe<CodeTestResultResponseEvent>(
                "code_execution.exchange",
                _responseQueueName, 
                "compiler.info.response",
                async (response) =>
                {
                    

                    if (_pendingRequests.TryGetValue(response.CorrelationId, out var tcs))
                    {
                        
                        tcs.TrySetResult(response);
                    }
                    else
                    {
                       
                    }
                });
            _isSubscribed = true;
            
        }
        catch (Exception ex)
        {
           
            throw;
        }
    }

    public async Task<List<TestExecutionResult>> ExecuteAllTestsAsync(string compiledCode,
        List<TestCaseInfo> testCasesDto,
        LanguageInfo languageId)
    {
        var request = new CodeSubmissionEvent()
        {
            Code = compiledCode,
            Language = languageId,
            TestCases = testCasesDto.Select(x => new TestCaseEvent()
            {
                Input = x.Input,
                Output = x.Output
            }).ToList(),
            // Указываем конкретную очередь для ответа
            ReplyToQueue = _responseQueueName,
            CorrelationId = Guid.NewGuid().ToString()
        };

        var tcs = new TaskCompletionSource<CodeTestResultResponseEvent>();
        _pendingRequests[request.CorrelationId] = tcs;

        try
        {
           

            _messageBus.Publish(
                request,
                "code_execution.exchange",
                "compiler.info.request");

            

            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
               
                throw new TimeoutException("Compiler request timeout");
            }

            var response = await tcs.Task;
            

            if (!response.Success)
            {
                throw new InvalidOperationException($"Compilation failed: {response.ErrorMessage}");
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
                        0
                    ));
                }
            }

            
                
            return results;
        }
        finally
        {
            _pendingRequests.TryRemove(request.CorrelationId, out _);
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