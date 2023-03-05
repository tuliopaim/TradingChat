using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TradingChat.Core.Messaging;

namespace TradingChat.Infrastructure.Messaging;

public class QueueCreator
{
    private readonly IServiceProvider _serviceProvider;

    public QueueCreator(IServiceProvider serviceProvider)
	{
        _serviceProvider = serviceProvider;
    }

    public async Task CreateQueues()
    {
        var environment = _serviceProvider.GetRequiredService<IHostingEnvironment>();

        if (environment.EnvironmentName is not ("Internal" or "Development"))
        {
            return;
        }

        if (environment.IsEnvironment("Internal"))
        {
            await Task.Delay(5 * 1000);
        }

        using var scope = _serviceProvider.CreateScope();

        var rabbitConnection = _serviceProvider.GetRequiredService<RabbitMqConnection>();

        using var channel = rabbitConnection.Connection.CreateModel();

        foreach(var routingKey in RabbitRoutingKeys.All)
        {
            channel.QueueDeclare(queue: routingKey,
               durable: true,
               exclusive: false,
               autoDelete: false,
               arguments: null);
        }
    }
}
