namespace SnookerLive;

public static class ServiceExtensions
{
    public static IServiceCollection AddRankingService(this IServiceCollection services)
    {
        services.AddScoped<IRankingService, RankingService>();
        return services;
    }
}