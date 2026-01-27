using SnookerOrg.Enums;
using SnookerOrg.Messages;

namespace SnookerOrg.Queues;

public sealed class QueueService
{
    private readonly LinkedList<QueueMessage> queue;
    private readonly object _lock = new();
    private readonly SemaphoreSlim _signal = new(0);
    public QueueService()
    {
        queue = new LinkedList<QueueMessage>();
    }
    public void Enqueue(Priority priority, SnookerOrgMessage snookerOrgMessage)
    {
        var message = new QueueMessage(priority, snookerOrgMessage);
        lock (_lock)
        {
            if(queue.Count == 0)
            {
                queue.AddFirst(message);
            }
            if(priority == Priority.RealTime)
            {
                queue.AddFirst(message);
            }else{
                var node = queue.First;
                while(node != null && node.Value.Priority <= priority)
                {
                    node = node.Next;
                }
                if(node == null)
                    queue.AddLast(message);
                else
                    queue.AddBefore(node, message);
            }
        }
        _signal.Release();
    }
    public async Task<QueueMessage> Dequeue(CancellationToken ct)
    {
        await _signal.WaitAsync(ct);
        lock (_lock)
        {
            var message = queue.First!;
            queue.RemoveFirst();
            return message.Value;
        }
    }
    public int Count
    {
        get {
            lock(_lock)
                return queue.Count;
        }
    }
}