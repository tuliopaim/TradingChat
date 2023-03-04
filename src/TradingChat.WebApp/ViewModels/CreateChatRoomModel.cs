using FluentValidation;

namespace TradingChat.WebApp.ViewModels;

public class CreateChatRoomModel
{
    public string? Name { get; set; }

    public int MaxNumberOfUsers { get; set; }
}

public class CreateChatRoomModelValidator : AbstractValidator<CreateChatRoomModel>
{
    public CreateChatRoomModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.MaxNumberOfUsers)
            .NotEmpty()
            .InclusiveBetween(2, 30);
    }
}

