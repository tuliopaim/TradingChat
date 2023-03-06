namespace TradingChat.Core.Messaging;

public interface IQueueConsumer
{
    void StartConsuming<TMessage>(
        string queueName,
        ushort prefetchCount,
        Func<TMessage, Task<bool>> messageHandler,
        ushort? maxRetry = null);
}

