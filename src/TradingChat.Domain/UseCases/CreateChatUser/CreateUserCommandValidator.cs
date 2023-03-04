using FluentValidation;

namespace TradingChat.Domain.UseCases.CreateChatUser;

public class CreateUserCommandValidator : AbstractValidator<CreateChatUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(100);
    }
}
