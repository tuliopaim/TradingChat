using TradingChat.ChatBot;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddSerilog()
    .AddTracing();

builder.Services.InjectServices(builder.Configuration);

builder.Services.AddHostedService<MessageCommandConsumer>();

var app = builder.Build();

app.Run();

