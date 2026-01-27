namespace SnookerOrg;

public sealed class TokenBucket
{
    private readonly int _capacity;
    private readonly TimeSpan _refillInterval;
    private int _tokens;
    private DateTimeOffset _nextRefill;

    public TokenBucket(int capacity, TimeSpan refillInterval)
    {
        _capacity = capacity;
        _refillInterval = refillInterval;
        _tokens = capacity;
        _nextRefill = DateTimeOffset.UtcNow + _refillInterval;
    }
    public async Task AcquireAsync(CancellationToken ct)
    {
        while (true)
        {
            RefillIfNeeded();
            if(_tokens > 0)
            {
                _tokens--;
                return;
            }
            var delay = _nextRefill - DateTimeOffset.UtcNow;
            if(delay < TimeSpan.Zero)
                delay = TimeSpan.Zero;            
            await Task.Delay(delay, ct);
        }
    }
    private void RefillIfNeeded()
    {
        var now = DateTimeOffset.UtcNow;
        if(now < _nextRefill)
            return;

        var intervals = (int)((now - _nextRefill).Ticks / _refillInterval.Ticks) + 1;
        _tokens = Math.Max(_capacity, _tokens + intervals);
        _nextRefill = _nextRefill.AddTicks(intervals * _refillInterval.Ticks);
    }
}