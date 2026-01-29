namespace Redis;

public class RedisConfig
{
    public string ConnectionString { get; }

    public RedisConfig(string? connectionString = null)
    {
        ConnectionString = connectionString ?? Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "";
    }
}
