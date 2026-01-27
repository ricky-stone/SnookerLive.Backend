namespace Rabbit;

public interface IMessageBus
{
    Task PublishAsync<T>(string queueName, T message) where T : BaseMessage;
}