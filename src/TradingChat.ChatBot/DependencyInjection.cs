using TradingChat.ChatBot.Commands.ChatMessageCommands;
using TradingChat.ChatBot.Commands.Contracts;
using TradingChat.ChatBot.Commands;
using TradingChat.ChatBot.External.Stooq;
using RabbitMQ.Client;
using TradingChat.Core.Messaging;
using TradingChat.Core.Rabbit;

namespace TradingChat.ChatBot;

public static class DependencyInjection
{
    public static IServiceCollection InjectServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.InjectRabbit(configuration);

        services.AddHttpClient<StooqClient>();

        services.AddScoped<IStockPriceService, StooqService>();

        services.AddScoped<ChatMessageCommandInvoker>();

        services.AddScoped<IChatMessageCommand, StockPriceCommand>();
        services.AddScoped<IChatMessageCommand, PingPongCommand>();

        return services;
    }

    public static void InjectRabbit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection(nameof(RabbitMqSettings))!;

        var rabbitSettings = configurationSection.Get<RabbitMqSettings>()!;

        services.Configure<RabbitMqSettings>(configurationSection);

        services.AddSingleton<IConnectionFactory>(x =>
            new ConnectionFactory
            {
                HostName = rabbitSettings.HostName,
                Port = rabbitSettings.Port,
                UserName = rabbitSettings.UserName,
                Password = rabbitSettings.Password,
                DispatchConsumersAsync = true,
                ConsumerDispatchConcurrency = 1,
                UseBackgroundThreadsForIO = false
            });

        services.AddSingleton<RabbitMqConnection>();
        services.AddSingleton<IQueueConsumer, RabbitMqConsumer>();

        services.AddScoped<IMessageProducer, RabbitMqProducer>();
    }
}
