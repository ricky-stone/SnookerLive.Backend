namespace SnookerLive;

public static class ServiceExtensions
{
    public static IServiceCollection AddRoundService(this IServiceCollection services)
    {
        services.AddScoped<IRoundService, RoundService>();
        return services;
    }
}