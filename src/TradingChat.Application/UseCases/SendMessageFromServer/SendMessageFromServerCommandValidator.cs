using FluentValidation;

namespace TradingChat.Application.UseCases.SendMessageFromServer;

public class SendMessageFromServerCommandValidator : AbstractValidator<SendMessageFromServerCommand>
{
    public SendMessageFromServerCommandValidator()
    {
        RuleFor(x => x.ChatRoomId).NotEmpty();

        RuleFor(x => x.Message)
            .NotEmpty()
            .Length(1, 1000);
    }
}
