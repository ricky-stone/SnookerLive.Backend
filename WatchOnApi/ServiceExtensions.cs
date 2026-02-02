namespace SnookerLive;

public static class ServiceExtensions
{
    public static IServiceCollection AddWatchOnService(this IServiceCollection services)
    {
        services.AddScoped<IWatchOnService, WatchOnService>();
        return services;
    }
}