using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace TradingChat.ChatBot;

public static class ObservabilityConfiguration
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.WithProperty("app", nameof(ChatBot))
            .Enrich.WithProperty("env", builder.Environment.EnvironmentName)
            .CreateLogger();

        builder.Logging.AddSerilog(logger);

        return builder;
    }

    public static void AddTracing(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            return;
        }

        var serviceName = $"{builder.Environment.ApplicationName}:{builder.Environment.EnvironmentName}";
        builder.Services.AddOpenTelemetry()
            .WithTracing(b => b
                .AddSource(builder.Environment.ApplicationName)
                .ConfigureResource(res => res.AddService(serviceName: serviceName))
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri(
                        builder.Configuration["OpenTelemetry:Endpoint"]
                        ?? throw new InvalidOperationException("OpenTelemetry:Endpoint is not configured"));
                    opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                }));
    }
}
