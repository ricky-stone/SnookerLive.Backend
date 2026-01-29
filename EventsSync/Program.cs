using EventsSync;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
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
        services.AddHostedService<Service>();
        services.AddRabbit();
    })
    .RunConsoleAsync();
