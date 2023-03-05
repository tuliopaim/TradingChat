namespace TradingChat.Core.Messaging;

public interface IMessageProducer
{
    bool Publish<TMessage>(TMessage message, string routingKey);
}
