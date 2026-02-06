using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace SnookerLive;

public static class HttpClientExtensions
{
    public static IServiceCollection AddPlayerHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IPlayerApiClient, PlayerApiClient>(c =>
            {
                c.BaseAddress = new Uri("http://player-api", UriKind.Absolute);
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