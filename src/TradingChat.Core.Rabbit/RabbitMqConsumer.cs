using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TradingChat.Core.Messaging;

namespace TradingChat.Core.Rabbit;

public class RabbitMqConsumer : IQueueConsumer
{
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly RabbitMqConnection _rabbitConnection;

    public RabbitMqConsumer(
        ILogger<RabbitMqConsumer> logger,
        RabbitMqConnection connection)
    {
        _logger = logger;
        _rabbitConnection = connection;
    }

    public void StartConsuming<TMessage>(
        string queueName,
        ushort prefetchCount,
        Func<TMessage, Task<bool>> messageHandler)
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
                    channel.BasicAck(args.DeliveryTag, multiple: false);
                    return;
                }

                channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing message type {MessageType}", typeof(TMessage).Name);

                channel.BasicNack(args.DeliveryTag, multiple: false, requeue: true);
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
    }
}

