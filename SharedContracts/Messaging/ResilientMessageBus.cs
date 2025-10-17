using Microsoft.Extensions.Logging;

namespace SharedContracts.Messaging;

using Polly;
using RabbitMQ.Client.Exceptions;


public class ResilientMessageBus : IMessageBus
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ResilientMessageBus> _logger;

    public ResilientMessageBus(IMessageBus messageBus, ILogger<ResilientMessageBus> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
    }

    public void Publish<T>(T message, string exchangeName, string routingKey) where T : class
    {
        var retryPolicy = Policy
            .Handle<BrokerUnreachableException>()
            .Or<OperationInterruptedException>()
            .WaitAndRetry(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, 
                        "Failed to publish message to {Exchange}. Retry {RetryCount} in {TimeSpan}s", 
                        exchangeName, retryCount, timeSpan.TotalSeconds);
                });

        retryPolicy.Execute(() => 
            _messageBus.Publish(message, exchangeName, routingKey));
    }

    public void Subscribe<T>(string exchangeName, string queueName, string routingKey, Func<T, Task> handler) where T : class
    {
        _messageBus.Subscribe(exchangeName, queueName, routingKey, handler);
    }

    public void Dispose()
    {
        _messageBus?.Dispose();
    }
}