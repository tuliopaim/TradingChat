using Serilog;

namespace TradingChat.WebApp.Configurations;

public static class LoggingConfiguration
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.WithProperty("Application", nameof(WebApp))
            .CreateLogger();

        builder.Logging.AddSerilog(logger);

        return builder;
    }
}

