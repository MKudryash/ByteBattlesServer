using System.Collections.Concurrent;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.SolutionService.Domain.Models;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;

public class RabbitMqCompilationService : ICompilationService, IDisposable
{
    private readonly IMessageBus _messageBus;
    private readonly IMemoryCache _cache;
    private readonly ILogger<RabbitMqCompilationService> _logger;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<CodeTestResultResponseEvent>> _pendingRequests = new();
    private readonly string _responseQueueName;
    private bool _disposed = false;

    public RabbitMqCompilationService(
        IMessageBus messageBus,
        IMemoryCache cache,
        ILogger<RabbitMqCompilationService> logger)
    {
        _messageBus = messageBus;
        _cache = cache;
        _logger = logger;
        
        // Создаем уникальное имя очереди для этого сервиса
        _responseQueueName = $"solution.compiler.responses.{Guid.NewGuid():N}";
        
        SubscribeToResponses();
    }

    private void SubscribeToResponses()
    {
        try
        {
        
           
            // В RabbitMqCompilationService в обработчике:
            _messageBus.Subscribe<CodeTestResultResponseEvent>(
                "code_execution.exchange",
                _responseQueueName, 
                "compiler.info.response", 
                async (response) =>
                {
                    _logger.LogInformation($"📥 [Solution] Received response for CorrelationId: {response.CorrelationId}");
        
                    if (_pendingRequests.TryGetValue(response.CorrelationId, out var tcs))
                    {
                        _logger.LogInformation($"✅ [Solution] Found pending request, setting result");
                        _logger.LogInformation(response.Success.ToString());
                        tcs.TrySetResult(response);
                    }
                    else
                    {
                        _logger.LogWarning($"⚠️ [Solution] No pending request found for CorrelationId: {response.CorrelationId}");
                    }
                });

                
            _logger.LogInformation($"🟢 [CompilationService] Successfully subscribed to responses");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"❌ [CompilationService] Failed to subscribe to responses");
            throw;
        }
    }

    public async Task<List<TestExecutionResult>> ExecuteAllTestsAsync(
        string compiledCode,
        List<TestCaseDto> testCasesDto,
        LanguageInfo languageInfo,
        List<LibraryInfo> libraries,
        string patternMain)
    {
  
        
        var request = new CodeSubmissionEvent()
        {
            Code = compiledCode,
            Language = languageInfo,
            TestCases = testCasesDto.Select(x => new TestCaseEvent()
            {
                Input = x.Input,
                Output = x.Output
            }).ToList(),
            PatternMain = patternMain,
            Libraries = libraries,
            ReplyToQueue = _responseQueueName,
            ReplyToRoutingKey = "compiler.info.response",
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
                throw new TimeoutException("Compiler request timeout (30 seconds)");
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

            
                Console.WriteLine(results.Count().ToString());
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