using RabbitMQ.Client;

namespace TradingChat.Core.Rabbit;

public static class RabbitMqExtensions
{
    public static int? GetRetryCount(this IBasicProperties properties)
    {
        if (properties.Headers is null) return null;

        return properties.Headers.TryGetValue("x-retry-count", out var retryCountObj)
            ? (int)retryCountObj
            : null;
    }

    public static IBasicProperties CreateRetryCountHeader(this IBasicProperties properties)
    {
        properties.Headers = new Dictionary<string, object>
        {
            {"x-retry-count", 0}
        };

        return properties;
    }

    public static IBasicProperties IncrementRetryCountHeader(this IBasicProperties properties)
    {
        var retryCount = properties.GetRetryCount();

        properties.Headers["x-retry-count"] = ++retryCount;

        return properties;
    }
}
