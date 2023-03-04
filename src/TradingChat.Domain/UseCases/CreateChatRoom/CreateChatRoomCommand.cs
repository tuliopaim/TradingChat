using TradingChat.Domain.UseCases.Base;

namespace TradingChat.Domain.UseCases.CreateChatRoom;

public record CreateChatRoomCommand(
    string Name,
    int MaxNumberOfUsers) : ICommand;
