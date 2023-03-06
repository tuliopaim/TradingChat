using Serilog;

namespace TradingChat.ChatBot;

public static class LoggingConfiguration
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.WithProperty("Application", nameof(ChatBot))
            .CreateLogger();

        builder.Logging.AddSerilog(logger);

        return builder;
    }
}
