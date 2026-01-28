namespace FrameApi;

public static class ServiceExtensions
{
    public static IServiceCollection AddFrameService(this IServiceCollection services)
    {
        services.AddScoped<IFrameService, FrameService>();
        return services;
    }
}