﻿using TradingChat.Application.Abstractions;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Shared;

namespace TradingChat.Application.UseCases.JoinChatRoom;

public class JoinChatRoomCommandHandler : ICommandHandler<JoinChatRoomCommand>
{
    private readonly ICurrentUser _currentUser;
    private readonly IChatRoomRepository _chatRoomRepository;

    public JoinChatRoomCommandHandler(
        ICurrentUser currentUser,
        IChatRoomRepository chatRoomRepository)
    {
        _currentUser = currentUser;
        _chatRoomRepository = chatRoomRepository;
    }

    public async Task<Result> Handle(JoinChatRoomCommand request, CancellationToken cancellationToken)
    {
        var chatRoom = await _chatRoomRepository.GetWithUsers(request.ChatRoomId);

        if (chatRoom is null)
        {
            return new Error("ChatRoom not found!");
        }

        var result = chatRoom.AddUser(_currentUser.Id!.Value);

        if (!result.IsSuccess) return result;

        await _chatRoomRepository.SaveChangesAsync(cancellationToken);

        return result;
    }
}

