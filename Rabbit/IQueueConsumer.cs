namespace Rabbit;

public interface IQueueConsumer<T> where T : BaseMessage
{
    IDisposable Start(string queue, Func<T, Task> handle);
}