using TradingChat.Application.Abstractions;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;
using TradingChat.Domain.Shared;

namespace TradingChat.Application.UseCases.CreateChatRoom;

public class CreateChatRoomHandler : ICommandHandler<CreateChatRoomCommand>
{
    private readonly ICurrentUser _currentUser;
    private readonly IChatRoomRepository _chatRoomRepository;

    public CreateChatRoomHandler(
        ICurrentUser currentUser,
        IChatRoomRepository chatRoomRepository)
    {
        _currentUser = currentUser;
        _chatRoomRepository = chatRoomRepository;
    }

    public async Task<Result> Handle(
        CreateChatRoomCommand request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        if (await _chatRoomRepository.NameAlreadyTaken(request.Name))
        {
            return new Error("Name already taken!");
        }

        var chatRoom = new ChatRoom(name, request.MaxNumberOfUsers, _currentUser.Id!.Value);

        _chatRoomRepository.Add(chatRoom);

        await _chatRoomRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
