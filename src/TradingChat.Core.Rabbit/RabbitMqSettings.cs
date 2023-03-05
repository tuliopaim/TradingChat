namespace TradingChat.Core.Rabbit;

public class RabbitMqSettings
{
    public required string HostName { get; set; }
    public required string UserName { get; set; }
    public required int Port { get; set; }
    public required string Password { get; set; }
    public required RabbitMqRetrySettings RetrySettings { get; set; }
}

public class RabbitMqRetrySettings
{
    public required int Count { get; set; }
    public required int DurationInSeconds { get; set; }
}
