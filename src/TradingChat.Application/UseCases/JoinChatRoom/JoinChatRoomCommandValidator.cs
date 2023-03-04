using FluentValidation;

namespace TradingChat.Application.UseCases.JoinChatRoom;

public class JoinChatRoomCommandValidator : AbstractValidator<JoinChatRoomCommand>
{
    public JoinChatRoomCommandValidator()
    {
        RuleFor(x => x.ChatRoomId).NotEmpty();
    }
}

