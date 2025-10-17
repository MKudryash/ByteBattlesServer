using RabbitMQ.Client;

namespace ByteBattlesServer.SharedContracts.Messaging;

public interface IMessageBus
{
    void Publish<T>(T message, string exchangeName, string routingKey) where T : class;
    void Subscribe<T>(string exchangeName, string queueName, string routingKey, Func<T, Task> handler) where T : class;
    void Dispose();
}
