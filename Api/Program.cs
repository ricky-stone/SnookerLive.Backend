using Redis;
using SnookerLive;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPlayerHttpClient()
    .AddEventHttpClient()
    .AddRankingHttpClient()
    .AddWatchOnHttpClient()
    .AddRoundHttpClient()
    .AddRedisCache()
    .AddSingleton<IEventService, EventService>()
    .AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();