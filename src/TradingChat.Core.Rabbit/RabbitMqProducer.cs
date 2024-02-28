using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using TradingChat.Core.Messaging;

namespace TradingChat.Core.Rabbit;

public class RabbitMqProducer : IMessageProducer
{
    private static readonly ActivitySource ActivitySource = new(nameof(RabbitMqProducer));
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    private readonly RabbitMqConnection _rabbitConexao;
    private readonly ILogger<RabbitMqProducer> _logger;
    private readonly IModel _channel;

    public RabbitMqProducer(RabbitMqConnection rabbitConexao, ILogger<RabbitMqProducer> logger)
    {
        _rabbitConexao = rabbitConexao;
        _logger = logger;
        _channel = Connection.CreateModel();
    }

    private IConnection Connection => _rabbitConexao.Connection;

    public bool Publish<TMessage>(TMessage message, string routingKey)
    {
        var serializedMessage = JsonSerializer.Serialize(message);

        return PublishMessage(serializedMessage, routingKey);
    }

    private bool PublishMessage(string serializedMessage, string routingKey)
    {
        try
        {
            if (Connection is null) return false;

            var activityName = $"PublishMessage {routingKey}";
            using var activity = ActivitySource.StartActivity(activityName, ActivityKind.Producer);

            ActivityContext contextToInject = default;
            if (activity is not null)
            {
                contextToInject = activity.Context;
            }
            else
            {
                contextToInject = Activity.Current?.Context ?? default;
            }

            var props = _channel.CreateBasicProperties();

            props.Persistent = true;
            props.CreateRetryCountHeader();

            Propagator.Inject(
                new PropagationContext(contextToInject, Baggage.Current),
                props,
                InjectTraceContextIntoBasicProperties);

            AddMessagingTags(activity, routingKey);

            var messageBytes = Encoding.UTF8.GetBytes(serializedMessage);

            _channel.BasicPublish("",
                routingKey,
                props,
                messageBytes);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception captured on {Method} - {@DataPublished}",
                nameof(PublishMessage), new { serializedMessage, routingKey });

            return false;
        }
    }

    private void InjectTraceContextIntoBasicProperties(IBasicProperties props, string key, string value)
    {
        try
        {
            props.Headers ??= new Dictionary<string, object>();

            props.Headers[key] = value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to inject trace context.");
        }
    }

    private static void AddMessagingTags(Activity? activity, string routingKey)
    {
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination_kind", "queue");
        activity?.SetTag("messaging.rabbitmq.routing_key", routingKey);
    }
}
