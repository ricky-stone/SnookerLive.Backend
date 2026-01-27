using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Rabbit;
using SnookerOrg;

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
        services.AddHttpClient<SnookerOrgClient>(http =>
        {
            http.BaseAddress = new Uri("https://api.snooker.org/");
            http.DefaultRequestHeaders.TryAddWithoutValidation(
                "X-Requested-By", "RickyStone");
        });
        services.AddRabbit();
    })
    .RunConsoleAsync();