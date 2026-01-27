using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Rabbit;

public sealed class RabbitMessageBus(IConnection conn, RabbitConfig cfg) : IMessageBus
{
    public Task PublishAsync<T>(string queue, T message) where T : BaseMessage
    {
        using var ch = conn.CreateModel();
        var rk = queue;

        ch.ExchangeDeclare(cfg.Exchange, ExchangeType.Direct, durable: true);
        ch.QueueDeclare(rk, durable: true, exclusive: false, autoDelete: false);
        ch.QueueBind(rk, cfg.Exchange, rk);

        var json = JsonSerializer.Serialize(message, JsonDefaults.Options);
        var body = Encoding.UTF8.GetBytes(json);

        ch.BasicPublish(cfg.Exchange, rk, basicProperties: null, body);
        return Task.CompletedTask;
    }
}