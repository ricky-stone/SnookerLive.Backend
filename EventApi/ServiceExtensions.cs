namespace SnookerLive;

public static class ServiceExtensions
{
    public static IServiceCollection AddEventService(this IServiceCollection services)
    {
        services.AddScoped<IEventService, EventService>();
        return services;
    }
}