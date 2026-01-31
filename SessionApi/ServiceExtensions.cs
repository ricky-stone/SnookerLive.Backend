namespace SnookerLive;

public static class ServiceExtensions
{
    public static IServiceCollection AddSessionService(this IServiceCollection services)
    {
        services.AddScoped<ISessionService, SessionService>();
        return services;
    }
}