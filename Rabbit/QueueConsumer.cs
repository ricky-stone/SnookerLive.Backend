using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Rabbit;

public sealed class QueueConsumer<T>(IConnection conn, RabbitConfig cfg) : IQueueConsumer<T> where T : BaseMessage
{
    public IDisposable Start(string queue, Func<T, Task> handle)
    {
        var ch = conn.CreateModel();
        var q = queue;

        ch.ExchangeDeclare(cfg.Exchange, ExchangeType.Direct, durable: true);
        ch.QueueDeclare(q, durable: true, exclusive: false, autoDelete: false);
        ch.QueueBind(q, cfg.Exchange, q);
        ch.BasicQos(0, 1, false);

        var consumer = new AsyncEventingBasicConsumer(ch);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var msg = JsonSerializer.Deserialize<T>(ea.Body.Span, JsonDefaults.Options);
                if (msg is not null)
                    await handle(msg);

                ch.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch
            {
                ch.BasicReject(ea.DeliveryTag, requeue: true);
            }
        };

        ch.BasicConsume(q, autoAck: false, consumer);
        return new ChannelLease(ch);
    }

    private sealed class ChannelLease(IModel ch) : IDisposable
    {
        public void Dispose()
        {
            ch.Close();
            ch.Dispose();
        }
    }
}