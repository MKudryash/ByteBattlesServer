using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.SharedContracts.Messaging;

using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


public class RabbitMQMessageBus : IMessageBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQMessageBus> _logger;

    public RabbitMQMessageBus(RabbitMQSettings settings, ILogger<RabbitMQMessageBus> logger)
    {
        _settings = settings;
        _logger = logger;
        
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password,
            VirtualHost = settings.VirtualHost
        };
        _logger.LogInformation("Creating RabbitMQ connection");
        _logger.LogInformation($"{factory.HostName}:{factory.Port}");
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Publish<T>(T message, string exchangeName, string routingKey) where T : class
    {
        _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(
            exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: properties,
            body: body);
    }

    public void Subscribe<T>(string exchangeName, string queueName, string routingKey, Func<T, Task> handler) where T : class
    {
        _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true);
        _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queueName, exchangeName, routingKey);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<T>(json);

            if (message != null)
            {
                try
                {
                    await handler(message);
                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                }
            }
        };

        _channel.BasicConsume(queueName, autoAck: false, consumer);
    }


    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}