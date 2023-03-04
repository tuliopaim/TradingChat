using TradingChat.Application.Abstractions;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;
using TradingChat.Domain.Shared;

namespace TradingChat.Application.UseCases.SendMessage;

public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand>
{
    private readonly ICurrentUser _currentUser;
    private readonly IChatRoomRepository _chatRoomRepository;

    public SendMessageCommandHandler(
        ICurrentUser currentUser,
        IChatRoomRepository chatRoomRepository)
    {
        _currentUser = currentUser;
        _chatRoomRepository = chatRoomRepository;
    }

    public async Task<Result> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var chatRoom = await _chatRoomRepository.GetWithUsersAsTracking(request.ChatRoomId);

        if (chatRoom is null)
        {
            return Result.WithError("Chat Room not found!");
        }

        if (!chatRoom.ContainsUser(_currentUser.Id!.Value))
        {
            return Result.WithError("User not allowed to send messages!");
        }
        
        var message = new ChatMessage(
            request.Message,
            _currentUser.Id!.Value,
            request.ChatRoomId);

        chatRoom.AddMessage(message);

        await _chatRoomRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
