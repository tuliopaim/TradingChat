using TradingChat.Domain.UseCases.Base;

namespace TradingChat.Domain.UseCases.GetChatsInfo;

public record struct GetChatsInfoQuery : IQuery<ChatsInfoResult>;
