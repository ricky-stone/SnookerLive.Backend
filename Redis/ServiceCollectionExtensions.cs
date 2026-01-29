using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Redis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(new RedisConfig(connectionString));
        
        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var config = serviceProvider.GetRequiredService<RedisConfig>();
            return ConnectionMultiplexer.Connect(config.ConnectionString);
        });
        
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        return services;
    }
    
    public static IServiceCollection AddRedisCache(this IServiceCollection services)
    {
        services.AddSingleton<RedisConfig>();
        
        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var config = serviceProvider.GetRequiredService<RedisConfig>();
            return ConnectionMultiplexer.Connect(config.ConnectionString);
        });
        
        services.AddSingleton<ICacheService, RedisCacheService>();
        
        return services;
    }
}
