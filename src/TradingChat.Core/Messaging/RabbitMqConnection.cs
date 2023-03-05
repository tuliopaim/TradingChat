using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace TradingChat.Core.Messaging;

public class RabbitMqConnection
{
    private readonly RabbitMqSettings _rabbitSettings;
    private readonly Lazy<IConnection> _lazyConnection;

    public RabbitMqConnection(IOptions<RabbitMqSettings> rabbitSettings)
    {
        _lazyConnection = new(CriarConexao);
        _rabbitSettings = rabbitSettings.Value;
    }

    public IConnection Connection => _lazyConnection.Value;

    private IConnection CriarConexao()
    {
        IConnectionFactory factory = new ConnectionFactory
        {
            HostName = _rabbitSettings.HostName,
            Port = _rabbitSettings.Port,
            UserName = _rabbitSettings.UserName,
            Password = _rabbitSettings.Password,
            DispatchConsumersAsync = true,
            ConsumerDispatchConcurrency = 1,
            UseBackgroundThreadsForIO = false
        };

        return factory.CreateConnection();
    }
}
