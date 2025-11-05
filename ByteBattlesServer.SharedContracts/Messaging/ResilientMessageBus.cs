using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace ByteBattlesServer.SharedContracts.Messaging;

public class ResilientMessageBus : IMessageBus
{
    private readonly Lazy<IMessageBus> _lazyMessageBus;
    private readonly ILogger<ResilientMessageBus> _logger;
    private const int MaxRetries = 5;
    private readonly TimeSpan _initialDelay = TimeSpan.FromSeconds(2);

    public ResilientMessageBus(RabbitMQSettings settings, ILogger<RabbitMQMessageBus> rabbitMqLogger, ILogger<ResilientMessageBus> logger)
    {
        _logger = logger;
        _lazyMessageBus = new Lazy<IMessageBus>(() => 
            new RabbitMQMessageBus(settings, rabbitMqLogger));
    }

    public void Publish<T>(T message, string exchange, string routingKey) where T : class
    {
        ExecuteWithRetry(() => _lazyMessageBus.Value.Publish(message, exchange, routingKey));
    }

    public void Subscribe<T>(string exchange, string queue, string routingKey, Func<T, Task> handler) where T : class
    {
        ExecuteWithRetry(() => _lazyMessageBus.Value.Subscribe(exchange, queue, routingKey, handler));
    }

    private void ExecuteWithRetry(Action action)
    {
        var retryCount = 0;
        var delay = _initialDelay;

        while (retryCount < MaxRetries)
        {
            try
            {
                action();
                return;
            }
            catch (BrokerUnreachableException ex)
            {
                retryCount++;
                _logger.LogWarning(ex,
                    "Failed to connect to RabbitMQ. Attempt {RetryCount}/{MaxRetries}. Retrying in {Delay} seconds...",
                    retryCount, MaxRetries, delay.TotalSeconds);

                if (retryCount >= MaxRetries)
                {
                    _logger.LogError(ex, "Failed to connect to RabbitMQ after {MaxRetries} attempts.", MaxRetries);
                    throw;
                }

                Thread.Sleep(delay);
                delay = TimeSpan.FromSeconds(delay.TotalSeconds * 1.5); // Exponential backoff
            }
        }
    }

    public void Dispose()
    {
        if (_lazyMessageBus.IsValueCreated)
        {
            _lazyMessageBus.Value.Dispose();
        }
    }
}