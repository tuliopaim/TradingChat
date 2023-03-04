using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TradingChat.Domain.Contracts;
using TradingChat.Domain.Entities;
using TradingChat.Domain.Shared;
using TradingChat.Domain.UseCases.Base;

namespace TradingChat.Domain.UseCases.CreateUser;

public class CreateChatUserHandler : ICommandHandler<CreateUserCommand>
{
    private readonly ILogger<CreateChatUserHandler> _logger;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly IChatUserRepository _userRepository;

    public CreateChatUserHandler(
        ILogger<CreateChatUserHandler> logger,
        UserManager<IdentityUser<Guid>> userManager,
        IChatUserRepository userRepository)
    {
        _logger = logger;
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        await _userRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await CreateUser(command, cancellationToken);

            if (!result.IsSuccess) return result;

            await _userRepository.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating user {UserEmail}..", command.Email);
            await _userRepository.RollbackTransactionAsync(cancellationToken);

            return Result.WithError("Error creating user");
        }
    }

    private async Task<Result> CreateUser(CreateUserCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating identity user {UserEmail}", command.Email);

        var identityUser = new IdentityUser<Guid>
        {
            Id = Guid.NewGuid(),
            UserName = command.Email,
            Email = command.Email,
        };

        var result = await _userManager.CreateAsync(identityUser, command.Password);

        if (!result.Succeeded)
        {
            await _userRepository.RollbackTransactionAsync(cancellationToken);

            return LogAndReturnUserManagerErrors(command, result);
        }

        _logger.LogInformation("IdentityUser {UserEmail} created!, creating ChatUser", command.Email);

        var chatUser = new ChatUser(command.Name, identityUser.Id);

        _userRepository.Add(chatUser);

        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private Result LogAndReturnUserManagerErrors(CreateUserCommand command, IdentityResult result)
    {
        var errors = result.Errors
           .Select(e => new Error(e.Description))
           .ToList();

        _logger.LogInformation("Error creating IdentityUser {UserEmail} - {@Errors}", command.Email, errors);

        return Result.WithErrors(errors);
    }
}
