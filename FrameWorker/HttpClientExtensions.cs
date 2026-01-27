using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace FrameWorker;

public static class HttpClientExtensions
{
    public static IServiceCollection AddFrameHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IFrameApiClient, FrameApiClient>(c =>
            {
                c.BaseAddress = new Uri("http://frame-api", UriKind.Absolute);
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