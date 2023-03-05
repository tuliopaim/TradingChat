using RabbitMQ.Client;
using TradingChat.ChatCommandBot;
using TradingChat.ChatCommandBot.Commands.Contracts;
using TradingChat.ExternalService.Stooq;
using TradingChat.ChatCommandBot.Commands;
using TradingChat.ChatCommandBot.Commands.ChatMessageCommands;
using TradingChat.Core.Rabbit;
using TradingChat.Core.Messaging;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostBuilder, services) =>
    {
        Inject(services, hostBuilder.Configuration);

        services.AddHostedService<MessageCommandConsumer>();
    })
    .Build();

host.Run();

static IServiceCollection Inject(
    IServiceCollection services,
    IConfiguration configuration)
{
    InjectRabbit(services, configuration);

    services.AddHttpClient<StooqClient>();
    services.AddScoped<IStockPriceService, StooqService>();

    services.AddScoped<ChatMessageCommandInvoker>();
    services.AddScoped<IChatMessageCommand, StockPriceCommand>();

    return services;
}

static void InjectRabbit(IServiceCollection services, IConfiguration configuration)
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
