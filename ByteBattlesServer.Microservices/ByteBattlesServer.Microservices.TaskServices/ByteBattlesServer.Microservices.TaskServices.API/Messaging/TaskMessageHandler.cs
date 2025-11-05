using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.API.Messaging;

public class TaskMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<LanguageMessageHandler> _logger;
    
    public TaskMessageHandler(
        IServiceProvider serviceProvider,
        IMessageBus messageBus,
        ILogger<LanguageMessageHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _messageBus = messageBus;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageBus.Subscribe<TestCaseInfoRequest>(
            "testcase.exchange",
            "task_services.testcase.requests",
            "testcase.info.request",
            HandleTestCasesInfoRequest);
        
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("TaskMessageHandler was cancelled");
        }
    }

    private async Task HandleTestCasesInfoRequest(TestCaseInfoRequest arg)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        try
        {
            var query = new GetTestCasesByTaskQuery(arg.TaskId);
            var testCase = await mediator.Send(query);

            var response =
                new TestCasesInfoResponse()
                {
                    TestCases = testCase.Select(x => new TestCaseInfo()
                        {
                            Input = x.Input,
                            Output = x.Output,
                            TaskId = arg.TaskId  
                        }
                    ).ToList(),
                    CorrelationId = arg.CorrelationId,
                    Success = true
                };
            
            _messageBus.Publish(
                response,
                "testcase.exchange",
                "testcase.info.response");  // ← Измените на этот routing key!


        }
        catch (Exception ex)
        {
            var errorResponse = new TestCaseInfoResponse()
            {
                TaskId = arg.TaskId,
                CorrelationId = arg.CorrelationId,
                Success = false,
                ErrorMessage = ex.Message
            };

            _messageBus.Publish(errorResponse, 
                "testcase.exchange",
                "testcase.info.response");  // ← И здесь тоже!

        }
    }
}