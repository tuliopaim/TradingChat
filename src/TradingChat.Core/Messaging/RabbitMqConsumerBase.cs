using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TradingChat.Core.Messaging;

public abstract class RabbitMqConsumerBase : BackgroundService
{
    private readonly ILogger<RabbitMqConsumerBase> _logger;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly ushort _prefetchCount;
    private readonly uint _prefetchSize = 0;

    protected RabbitMqConsumerBase(
        RabbitMqConnection rabbitConnection,
        ILogger<RabbitMqConsumerBase> logger,
        string queueName,
        ushort prefetchCount = 1)
    {
        ArgumentNullException.ThrowIfNull(queueName, nameof(queueName));

        _logger = logger;
        _queueName = queueName;
        _prefetchCount = prefetchCount;

        var connection = rabbitConnection.Connection;

        _channel = connection.CreateModel();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Consuming from [{QueueName}]", _queueName);

        StartConsumer();

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogDebug("Finish consuming [{QueueName}]", _queueName);
    }

    private void StartConsumer()
    {
        _logger.LogInformation("Starting consumer for queue {queueName}", _queueName);

        _channel.BasicQos(_prefetchSize, _prefetchCount, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += MessageReceived;

        _channel.BasicConsume(_queueName, false, consumer);
    }

    private async Task MessageReceived(object sender, BasicDeliverEventArgs rabbitEventArgs)
    {
        try
        {
            _logger.LogInformation("Message received on {ConsumerType}", GetType().Name);

            var result = await HandleMessage(rabbitEventArgs);

            if (!result)
            {
                _logger.LogError("Error processing message");
                return;
            }

            _channel.BasicAck(rabbitEventArgs.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception captured");
        }
    }

    protected abstract Task<bool> HandleMessage(BasicDeliverEventArgs rabbitEventArgs);
}
