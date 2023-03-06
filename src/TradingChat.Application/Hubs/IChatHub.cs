using TradingChat.Application.UseCases.Shared;

namespace TradingChat.Application.Hubs;

public interface IChatHub
{
    Task ReceiveMessage(ChatMessageInfoDto command);
}
