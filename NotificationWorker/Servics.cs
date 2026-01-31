using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;

namespace SnookerLive;

public sealed class Service : BackgroundService
{
    private readonly Dictionary<string, IController> _controllers = [];
    private readonly ILogger<Service> _logger;
    private readonly IQueueConsumer<NotificationMessage> _queue;

    public Service(ILogger<Service> logger, IQueueConsumer<NotificationMessage> queue,
        IEnumerable<IController> controllers)
    {
        _logger = logger;
        _queue = queue;
        foreach (var controller in controllers)
            _controllers[controller.Action] = controller;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _queue.Start("notifications", HandleMessage);
    }

    private async Task HandleMessage(NotificationMessage message)
    {
        _logger.LogInformation("Received message: {Action}", message.Action);

        if (string.IsNullOrEmpty(message.Data))
        {
            _logger.LogWarning("Message data is empty for action: {Action}", message.Action);
            return;
        }

        if (_controllers.TryGetValue(message.Action, out var controller))
        {
            if (!controller.Validate(message.Data))
            {
                _logger.LogWarning("Validation failed for message action: {Action}", message.Action);
                return;
            }

            await controller.HandleMessage(message.Data);
        }
        else
        {
            _logger.LogWarning("No controller found for action: {Action}", message.Action);
            return;
        }
    }
}