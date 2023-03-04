using FluentValidation;

namespace TradingChat.Domain.UseCases.CreateChatRoom;

public class CreateChatRoomCommandValidator : AbstractValidator<CreateChatRoomCommand>
{
    public CreateChatRoomCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.MaxNumberOfUsers)
            .NotEmpty()
            .InclusiveBetween(2, 30);
    }
}
