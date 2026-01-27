using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Rabbit;

public static class RabbitDi
{
    public static IServiceCollection AddRabbit(this IServiceCollection s)
    {
        s.AddSingleton<RabbitConfig>();
        s.AddSingleton<IConnection>(sp =>
        {
            var cfg = sp.GetRequiredService<RabbitConfig>();
            var factory = new ConnectionFactory
            {
                Uri = new Uri(cfg.ConnectionString),
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = true,
                ClientProvidedName = AppDomain.CurrentDomain.FriendlyName
            };
            return factory.CreateConnection();
        });
        s.AddSingleton<IMessageBus, RabbitMessageBus>();
        s.AddSingleton(typeof(IQueueConsumer<>), typeof(QueueConsumer<>));
        return s;
    }
}