using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Context;

namespace TradingChat.WebApp.Configurations;

public static class ObservabilityConfiguration
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

    public static WebApplicationBuilder AddTracing(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            return builder;
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

        return builder;
    }

    /// <summary>
    /// Enrich logs with traceId, so the log can be correlated with the trace.
    /// </summary>
    /// <param name="app">WebApplication</param>
    public static void EnrichLogsWithTraceId(this WebApplication app)
    {
        app.Use(async (_, next) =>
        {
            using (LogContext.PushProperty("TraceId", System.Diagnostics.Activity.Current?.TraceId.ToHexString() ?? string.Empty))
            {
                await next();
            }
        });
    }
}
