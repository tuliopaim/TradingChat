using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TradingChat.Core.Messaging;

namespace TradingChat.Core.Rabbit;

public class RabbitMqConsumer : IQueueConsumer
{
    private static readonly ActivitySource ActivitySource = new(nameof(RabbitMqConsumer));
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

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
            var parentContext = Propagator.Extract(
                    default,
                    args.BasicProperties,
                    ExtractTraceContextFromBasicProperties);

            Baggage.Current = parentContext.Baggage;

            var activityName = $"ReceiveMessage {args.RoutingKey}";

            using var activity = ActivitySource.StartActivity(
                activityName,
                ActivityKind.Consumer,
                parentContext.ActivityContext);

            _logger.LogInformation("Message of type {MessageType} received on {ConsumerType}",
                typeof(TMessage).Name,
                GetType().Name);

            try
            {
                var message = Encoding.UTF8.GetString(args.Body.Span);

                activity?.AddTag("message", message);

                var success = await messageHandler(JsonSerializer.Deserialize<TMessage>(message)!);

                if (success)
                {
                    _logger.LogInformation("Finished processing the message of type {MessageType}", typeof(TMessage).Name);

                    channel.BasicAck(args.DeliveryTag, multiple: false);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing message of type {MessageType}", typeof(TMessage).Name);
            }

            NackWithRetryIncrement<TMessage>(args, channel, maxRetry);
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

        _logger.LogInformation("Nack message of type {MessageType} for the {RetryCount} time...", typeof(TMessage), retryCount);

        if (shouldRetry)
        {
            _logger.LogInformation("Re-enqueuing message of type {MessageType} for retry...", typeof(TMessage));

            channel.BasicPublish(args.Exchange, args.RoutingKey, args.BasicProperties, args.Body);

            return;
        }

        channel.BasicNack(args.DeliveryTag, multiple: false, requeue: false);
    }

    private IEnumerable<string> ExtractTraceContextFromBasicProperties(IBasicProperties props, string key)
    {
        try
        {
            if (props.Headers.TryGetValue(key, out var value))
            {
                var bytes = value as byte[];
                return new[] { Encoding.UTF8.GetString(bytes!) };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract trace context.");
        }

        return Enumerable.Empty<string>();
    }
}
