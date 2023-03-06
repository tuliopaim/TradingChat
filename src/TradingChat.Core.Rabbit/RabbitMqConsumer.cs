using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TradingChat.Core.Messaging;

namespace TradingChat.Core.Rabbit;

public class RabbitMqConsumer : IQueueConsumer
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly RabbitMqConnection _rabbitConnection;
    private readonly RabbitMqSettings _rabbitSettings;

    public RabbitMqConsumer(
        ILogger<RabbitMqConsumer> logger,
        IOptions<RabbitMqSettings> rabbitSettings,
        RabbitMqConnection connection)
    {
        _logger = logger;
        _rabbitConnection = connection;
        _rabbitSettings = rabbitSettings.Value;
    }

    public void StartConsuming<TMessage>(
        string queueName,
        ushort prefetchCount,
        Func<TMessage, Task<bool>> messageHandler,
        ushort? maxRetry = null)
    {
        var channel = _rabbitConnection.Connection.CreateModel();

        channel.BasicQos(prefetchSize: 0, prefetchCount, global: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += async (sender, args) =>
        {
            _logger.LogInformation("Message of type {MessageType} received on {ConsumerType}",
                typeof(TMessage).Name,
                GetType().Name);

            try
            {
                var success = await messageHandler(args.GetDeserializedMessage<TMessage>());

                if (success)
                {
                    _logger.LogInformation("Finished processing the message of type {MessageType}", typeof(TMessage).Name);

                    channel.BasicAck(args.DeliveryTag, multiple: false);
                    return;
                }

                NackWithRetryIncrement<TMessage>(args, channel, maxRetry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing message of type {MessageType}", typeof(TMessage).Name);

                NackWithRetryIncrement<TMessage>(args, channel, maxRetry);
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
    }

    private void NackWithRetryIncrement<TMessage>(
        BasicDeliverEventArgs args,
        IModel channel,
        ushort? maxRetry)
    {
        args.BasicProperties.IncrementRetryCountHeader();

        var maxRetryCount = maxRetry ?? _rabbitSettings.RetrySettings.Count;

        var retryCount = args.BasicProperties.GetRetryCount();
        var shouldRetry = retryCount <= maxRetryCount;

        _logger.LogInformation("Nacking message of type {MessageType} for the {RetryCount} time...", typeof(TMessage), retryCount);

        if (shouldRetry)
        {
            _logger.LogInformation("Requeuing message of type {MessateType} for retry...", typeof(TMessage));

            channel.BasicPublish(args.Exchange, args.RoutingKey, args.BasicProperties, args.Body);
        }

        channel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
    }
}

