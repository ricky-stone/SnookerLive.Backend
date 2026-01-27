using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NotificationWorker;
using Rabbit;

await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Information);
        logging.AddSimpleConsole(o =>
        {
            o.SingleLine = true;
            o.TimestampFormat = "HH:mm:ss ";
            o.ColorBehavior = LoggerColorBehavior.Enabled;
            o.IncludeScopes = false;
        });
    })
    .ConfigureServices(services =>
    {
        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IController).IsAssignableFrom(t));
        foreach (var type in types)
        {
            services.AddSingleton(typeof(IController), type);
        }

        services.AddHostedService<Service>();
        services.AddRabbit();
    })
    .RunConsoleAsync();