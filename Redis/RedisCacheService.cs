using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace Redis;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan ttl) where T : class;
    Task RemoveAsync(string key);
    Task<T?> GetOrSetAsync<T>(string key, TimeSpan ttl, Func<Task<T?>> factory) where T : class;
}

public sealed class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var value = await _db.StringGetAsync(key);
        if (!value.HasValue) return null;

        return JsonSerializer.Deserialize<T>(value.ToString(), JsonOptions);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl) where T : class
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        return _db.StringSetAsync(key, json, ttl);
    }

    public Task RemoveAsync(string key)
        => _db.KeyDeleteAsync(key);

    public async Task<T?> GetOrSetAsync<T>(string key, TimeSpan ttl, Func<Task<T?>> factory) where T : class
    {
        var cached = await GetAsync<T>(key);
        if (cached is not null) return cached;

        var value = await factory();
        if (value is null) return null;

        await SetAsync(key, value, ttl);
        return value;
    }
}