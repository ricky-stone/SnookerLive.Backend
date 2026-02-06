namespace SnookerLive;

public static class ServiceExtensions
{
    public static IServiceCollection AddPlayerService(this IServiceCollection services)
    {
        services.AddScoped<IPlayerService, PlayerService>();
        return services;
    }
}