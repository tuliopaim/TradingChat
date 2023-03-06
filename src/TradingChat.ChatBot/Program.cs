using RabbitMQ.Client;
using TradingChat.Core.Rabbit;
using TradingChat.Core.Messaging;
using TradingChat.ChatBot.External.Stooq;
using TradingChat.ChatBot.Commands.ChatMessageCommands;
using TradingChat.ChatBot;
using TradingChat.ChatBot.Commands;
using TradingChat.ChatBot.Commands.Contracts;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostBuilder, services) =>
    {
        if (hostBuilder.HostingEnvironment.IsEnvironment("Internal"))
        {
            // wait for the environment to run
            Task.Delay(TimeSpan.FromSeconds(10)).GetAwaiter().GetResult();
        }

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
    services.AddScoped<IChatMessageCommand, PingPongCommand>();

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
