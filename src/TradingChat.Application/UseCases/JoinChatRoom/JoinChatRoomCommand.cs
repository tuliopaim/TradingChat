using TradingChat.Application.Abstractions;

namespace TradingChat.Application.UseCases.JoinChatRoom;

public record JoinChatRoomCommand(Guid ChatRoomId) : ICommand;

