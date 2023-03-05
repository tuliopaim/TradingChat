using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TradingChat.Core.Messaging;

public abstract class QueueConsumerBackgroundService<TMessage> : BackgroundService
{
    private readonly ILogger<QueueConsumerBackgroundService<TMessage>> _logger;
    private readonly IQueueConsumer _consumer;
    private readonly string _queueName;
    private readonly ushort _prefetchCount;

    public QueueConsumerBackgroundService(
        ILogger<QueueConsumerBackgroundService<TMessage>> logger,
        IQueueConsumer consumer,
        string queueName,
        ushort prefetchCount = 1)
    {
        _logger = logger;
        _consumer = consumer;
        _queueName = queueName;
        _prefetchCount = prefetchCount;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.StartConsuming<TMessage>(
            _queueName,
            _prefetchCount,
            HandleMessage);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogDebug("Finish consuming [{QueueName}]", _queueName);
    }

    protected abstract Task<bool> HandleMessage(TMessage message);
}


