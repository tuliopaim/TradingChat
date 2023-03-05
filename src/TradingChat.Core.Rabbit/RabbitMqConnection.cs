using Polly;
using Polly.Retry;
using RabbitMQ.Client;

namespace TradingChat.Core.Rabbit;

public class RabbitMqConnection
{
    private readonly IConnectionFactory _factory;

    private static readonly object _connectionLocker = new();

    public RabbitMqConnection(IConnectionFactory factory)
    {
        _factory = factory;
        _connection = GetConnection();
    }

    private IConnection _connection;
    public IConnection Connection => _retryPolicy.Execute(GetConnection);

    private IConnection GetConnection()
    {
        if (_connection is { IsOpen: true }) return _connection;
        lock (_connectionLocker)
        {
            if (_connection is { IsOpen: true }) return _connection;
            _connection?.Dispose();
            _connection = _factory.CreateConnection();

            return _connection;
        }
    }

    private static readonly IEnumerable<TimeSpan> _sleepsBetweenRetries = new List<TimeSpan>
    {
        TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(8),
    };

    private readonly RetryPolicy _retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetry(sleepDurations: _sleepsBetweenRetries);
}
