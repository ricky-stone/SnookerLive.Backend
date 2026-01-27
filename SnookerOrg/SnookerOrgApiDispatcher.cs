using System.Threading.Channels;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnookerOrg.Enums;

namespace SnookerOrg;

public sealed class SnookerOrgApiDispatcher : BackgroundService
{
    private readonly PriorityQueue<ApiRequest, int> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);
    private readonly object _lock = new();
    private readonly SnookerOrgClient _client;
    private readonly TokenBucketRateLimiter limiter;
    private readonly ILogger _logger;

    public SnookerOrgApiDispatcher(SnookerOrgClient client, ILogger<SnookerOrgApiDispatcher> logger)
    {
        _client = client;
        _logger = logger;
        limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = 1,
            TokensPerPeriod = 1,
            ReplenishmentPeriod = TimeSpan.FromSeconds(10.0 / 3.0),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = int.MaxValue,
            AutoReplenishment = true
        });
    }

    public Task<string> EnqueueAsync(Priority priority, CancellationToken ct, params object?[] url)
    {
        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        var request = new ApiRequest(url, tcs);
        lock (_lock)
        {
            _queue.Enqueue(request, (int)priority);
        }

        _signal.Release();
        return tcs.Task;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _signal.WaitAsync(stoppingToken);
            ApiRequest request;
            lock (_lock)
            {
                if (!_queue.TryDequeue(out request!, out _))
                    continue;
            }

            using var lease = await limiter.AcquireAsync(1, stoppingToken);
            try
            {
                var result = await CallWithRetry(request, stoppingToken);
                request.Tcs.SetResult(result);
            }
            catch (Exception e)
            {
                request.Tcs.SetException(e);
            }
        }
    }

    private async Task<string> CallWithRetry(ApiRequest request, CancellationToken ct)
    {
        const int maxRetries = 5;
        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            var response = await _client.Call(ct, request.Url);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return await response.Content.ReadAsStringAsync(ct);
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                if (attempt == maxRetries)
                    return string.Empty;

                var delay = (int)Math.Pow(2, attempt) * 500 + Random.Shared.Next(0, 250);

                _logger.LogWarning("403 Received, Retry {attempt}/{max}. Backing off {delay}ms", attempt + 1,
                    maxRetries, delay);
                await Task.Delay(delay, ct);
                using var lease = await limiter.AcquireAsync(1, ct);
                continue;
            }

            return string.Empty;
        }

        return string.Empty;
    }
}

internal sealed record ApiRequest(object?[] Url, TaskCompletionSource<string> Tcs);