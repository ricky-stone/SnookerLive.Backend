using System.Net;

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

    public static IServiceCollection AddRoundHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IRoundApiClient, RoundApiClient>(c =>
            {
                c.BaseAddress = new Uri("http://round-api", UriKind.Absolute);
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

    public static IServiceCollection AddWatchOnHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IWatchOnApiClient, WatchOnApiClient>(c =>
            {
                c.BaseAddress = new Uri("http://watchon-api", UriKind.Absolute);
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

    public static IServiceCollection AddRankingHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IRankingApiClient, RankingApiClient>(c =>
            {
                c.BaseAddress = new Uri("http://ranking-api", UriKind.Absolute);
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