using Microsoft.EntityFrameworkCore;

namespace SnookerLive;

public static class DatabaseExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<PlayerDbContext>(options => 
            options.UseMongoDB(connectionString, "Players"));

        return services;
    }

    public static async Task<WebApplication> EnsureIndexesCreatedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlayerDbContext>();
        await db.EnsureIndexesCreatedAsync();
        return app;
    }
}