using TradingChat.Application.Abstractions;

namespace TradingChat.Application.UseCases.GetChatsInfo;

public record struct GetChatsInfoQuery : IQuery<ChatsInfoResult>;
