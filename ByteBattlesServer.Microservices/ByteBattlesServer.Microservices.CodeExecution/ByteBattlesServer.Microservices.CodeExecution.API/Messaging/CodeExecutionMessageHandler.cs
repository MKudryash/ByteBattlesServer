using ByteBattlesServer.Microservices.CodeExecution.Application.Commands;
using ByteBattlesServer.Microservices.CodeExecution.Application.DTOs;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;

namespace ByteBattlesServer.Microservices.CodeExecution.API.Messaging;

public class CodeExecutionMessageHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<CodeExecutionMessageHandler> _logger;

    public CodeExecutionMessageHandler(
        IServiceProvider serviceProvider,
        IMessageBus messageBus,
        ILogger<CodeExecutionMessageHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _messageBus = messageBus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {


        try
        {
            _messageBus.Subscribe<CodeSubmissionEvent>(
                "code_execution.exchange",
                "code_execution_services.compiler.requests",
                "compiler.info.request",
                HandleCompilerRequest);

        }
        catch (Exception ex)
        {

            throw;
        }

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {

        }
    }

    private async Task HandleCompilerRequest(CodeSubmissionEvent arg)
    {


        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            var query = new TestCodeCommand(arg.Code,
                arg.Language,
                arg.TestCases.Select(x => new TestCaseDto()
                {
                    Input = x.Input,
                    ExpectedOutput = x.Output,
                }).ToList(),
                arg.Libraries,
                arg.PatternFunction,
                arg.PatternMain);


            var testCase = await mediator.Send(query);

            var response = new CodeTestResultResponseEvent()
            {
                AllTestsPassed = testCase.AllTestsPassed,
                Results = testCase.Results.Select(x => new TestCaseEvent()
                {
                    Input = x.Input,
                    Output = x.ExpectedOutput,
                    ActualOutput = x.ActualOutput,
                    ExecutionTime = x.ExecutionTime,
                    IsPassed = x.IsPassed
                }).ToList(),
                CorrelationId = arg.CorrelationId,
                Success = true
            };


            var routingKey = arg.ReplyToRoutingKey ?? "compiler.info.response";
            _messageBus.Publish(
                response,
                "code_execution.exchange",
                routingKey);
            _logger.LogInformation($"📤 Published response to routing key: {routingKey}, CorrelationId: {response.CorrelationId}");
            _logger.LogInformation($"📥 Received CodeSubmissionEvent. " +
                                   $"CorrelationId: {arg.CorrelationId}, " +
                                   $"ReplyToQueue: {arg.ReplyToQueue}, " +
                                   $"ReplyToRoutingKey: {arg.ReplyToRoutingKey}");

        }
        catch (Exception ex)
        {


            var errorResponse = new CodeTestResultResponseEvent()
            {
                CorrelationId = arg.CorrelationId,
                Success = false,
                ErrorMessage = ex.Message
            };

            _messageBus.Publish(errorResponse,
                "code_execution.exchange",
                "compiler.info.response");
        }
    }
}
