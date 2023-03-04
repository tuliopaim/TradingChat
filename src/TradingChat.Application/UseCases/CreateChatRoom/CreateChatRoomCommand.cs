using TradingChat.Application.Abstractions;

namespace TradingChat.Application.UseCases.CreateChatRoom;

public record CreateChatRoomCommand(
    string Name,
    int MaxNumberOfUsers) : ICommand;
