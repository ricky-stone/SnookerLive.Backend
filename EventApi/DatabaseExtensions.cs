using Microsoft.EntityFrameworkCore;

namespace SnookerLive;

public static class DatabaseExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EventDbContext>(options => 
            options.UseMongoDB(connectionString, "Events"));

        return services;
    }

    public static async Task<WebApplication> EnsureIndexesCreatedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EventDbContext>();
        await db.EnsureIndexesCreatedAsync();
        return app;
    }
}