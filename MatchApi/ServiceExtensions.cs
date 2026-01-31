namespace SnookerLive;

public static class ServiceExtensions
{
    public static IServiceCollection AddSessionService(this IServiceCollection services)
    {
        services.AddScoped<IMatchService, MatchService>();
        return services;
    }
}