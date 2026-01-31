using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace UpcomingEventMatchesSync;

public static class HttpClientExtensions
{
    public static IServiceCollection AddEventHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IEventApiClient, EventApiClient>(c =>
            {
                c.BaseAddress = new Uri("http://event-api", UriKind.Absolute);
                c.Timeout = TimeSpan.FromSeconds(3);
                c.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                c.DefaultRequestHeaders.Add("X-Internal", "true");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });

        return services;
    }
}