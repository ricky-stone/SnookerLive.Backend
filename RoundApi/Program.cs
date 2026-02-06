using SnookerLive;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING")
    ?? throw new InvalidOperationException("Missing required environment variable: MONGO_CONNECTION_STRING");

builder.Services
    .AddMongoDb(connectionString)
    .AddRoundService()
    .AddControllers();

var app = builder.Build();

await app.EnsureIndexesCreatedAsync();

app.UseAuthorization();
app.MapControllers();

app.Run();