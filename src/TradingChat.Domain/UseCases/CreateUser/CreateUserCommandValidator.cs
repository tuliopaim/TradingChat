using FluentValidation;

namespace TradingChat.Domain.UseCases.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
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
