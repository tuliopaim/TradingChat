using System.Diagnostics;

namespace TradingChat.WebApp.Application;

public static class Telemetry
{
    public static readonly ActivitySource MyActivitySource = new(nameof(WebApp));
}
