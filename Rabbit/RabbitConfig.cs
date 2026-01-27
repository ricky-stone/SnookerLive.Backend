namespace Rabbit;

public class RabbitConfig(string? connectionString = null, string? exchange = null)
{
    public string ConnectionString { get; } =
        connectionString ?? Environment.GetEnvironmentVariable("RABBIT_CONNECTION_STRING") ?? "";

    public string Exchange { get; } =
        exchange ?? Environment.GetEnvironmentVariable("RABBIT_EXCHANGE") ?? "";
}