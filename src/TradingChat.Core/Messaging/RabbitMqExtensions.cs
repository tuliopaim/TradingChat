using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace TradingChat.Core.Messaging;

public static class RabbitMqExtensions
{
    public static T GetDeserializedMessage<T>(this BasicDeliverEventArgs rabbitEventArgs)
    {
        var messageBody = GetMessageBody(rabbitEventArgs);
        return JsonSerializer.Deserialize<T>(messageBody)!;
    }

    private static string GetMessageBody(this BasicDeliverEventArgs rabbitEventArgs)
    {
        return Encoding.UTF8.GetString(rabbitEventArgs.Body.Span);
    }

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