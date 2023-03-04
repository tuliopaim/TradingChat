using FluentValidation;

namespace TradingChat.Application.UseCases.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ChatRoomId).NotEmpty();

        RuleFor(x => x.Message)
            .NotEmpty()
            .Length(1, 1000);
    }
}
